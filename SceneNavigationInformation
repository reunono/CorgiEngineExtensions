using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Collections;

namespace MoreMountains.CorgiEngine
{
    public class SceneNavigationInformation : MonoBehaviour
    {
        public static SceneNavigationInformation Instance;

        public List<WaypointInformation> NavigationWaypoints = new();
        public List<WaypointInformation> NavigationGaps = new();


        void Awake()
        {   // Singleton -------------------------
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
    }


    /// <summary>
    /// Determines the appropriate waypoint for the AI to navigate towards based on the relative positions
    /// of the owner and the target. This method considers various factors such as height differences,
    /// navigational gaps, and the presence of waypoints at the same height as the owner or target.
    /// 
    /// It requires that you setup an object with the above class (SceneNavigationInformation) and populate it with
    /// waypoints and gaps in the scene.
    /// 
    /// Waypoints are objects that the AI can navigate towards. Gaps are objects that the AI cannot navigate through.
    /// The AI will try to find a path to the target without any gaps.
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Navigate Towards Target")]
    public class AIAction_NavigateToTarget : AIAction
    {
        [Tooltip("The minimum distance to the target that this Character can reach")]
        public float MinimumDistance = 1f;

        [SerializeField, ReadOnly] protected AIDecision_DetectTargetLadder _detectTargetLadder;
        protected GK_CharacterLadder _gK_CharacterLadder;
        protected CharacterHorizontalMovement _characterHorizontalMovement;

        protected Transform _targetToMoveTowards;
        protected Transform _adjacentTargetWaypoint;
        protected Character _character;
        protected CorgiController _controller;

        protected bool _pathwayToTargetIsDefined = false;
        protected bool _nextLadderMoveIsUp = false;
        protected bool _avoidLadders = false;
        protected bool _climbing = false;
        protected bool _tempAvoidLadders = false;


        public override void Initialization()
        {
            if (!ShouldInitialize) return;

            _character = this.gameObject.GetComponentInParent<Character>();
            _gK_CharacterLadder = _character.FindAbility<GK_CharacterLadder>();
            _characterHorizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
            _controller = _character.GetComponent<CorgiController>();
            _brain.Owner = _character.gameObject;

            AddDetectTargetLadder_Script();
        }

        public override void PerformAction() // Gets called on Update
        {
            if (_targetToMoveTowards != _brain.Target && !_pathwayToTargetIsDefined && !_climbing)
            {
                FigureOutWayPoint();
            }

            if (_pathwayToTargetIsDefined && !_climbing)
            {
                MoveTowardsNextPathLocation();
            }

            if (_tempAvoidLadders || _avoidLadders) return; // -------------- Will avoid ladders for 1 second

            if (_gK_CharacterLadder.IsCollidingWithLadder())
            {
                bool ladderIsTarget = Mathf.Abs(_gK_CharacterLadder.GetLadder().transform.position.x - _targetToMoveTowards.position.x) < 2f;
                if (!ladderIsTarget) return;

                _climbing = true;
                _characterHorizontalMovement.SetHorizontalMove(0f);
                _gK_CharacterLadder.Execute_ClimbingProcessForAiBrain(up: _nextLadderMoveIsUp, _gK_CharacterLadder.GetLadder());
                _controller.CollisionsOff();
                // ^^ This is called here again, as well as in Execute_ above, because it is getting called off some place and I cant be bothered to find it
            }

            // Finish climbing   
            if (!_climbing) return; // --------------------- Only if climbing
            if (_nextLadderMoveIsUp)
            {
                //Debug.Log("Distance Up: " + Mathf.Abs(_brain.Owner.transform.position.y - _adjacentTargetWaypoint.transform.position.y));
                if (Mathf.Abs(_brain.Owner.transform.position.y - _adjacentTargetWaypoint.transform.position.y - 2) < 1f)
                {
                    _gK_CharacterLadder.Execute_EndClimbingProcessForAiBrain(_brain);
                    ResetActionBooleans();
                    StartCoroutine(AvoidLaddersTemporarily());
                    _climbing = false;
                }
            }
            else // if next ladder move is down
            {
                //Debug.Log("Distance Down: " + Mathf.Abs(_brain.Owner.transform.position.y - _adjacentTargetWaypoint.transform.position.y));
                if (Mathf.Abs(_adjacentTargetWaypoint.transform.position.y - _brain.Owner.transform.position.y) < 1f)
                {
                    _gK_CharacterLadder.Execute_EndClimbingProcessForAiBrain(_brain);
                    ResetActionBooleans();
                    StartCoroutine(AvoidLaddersTemporarily());
                    _climbing = false;
                }
            }
        }

        public virtual void ResetActionBooleans()
        {
            _targetToMoveTowards = null;
            _pathwayToTargetIsDefined = false;
        }

        protected virtual void FigureOutWayPoint()
        {
            List<WaypointInformation> navigationWaypoints = new(SceneNavigationInformation.Instance.NavigationWaypoints);
            List<WaypointInformation> navigationGaps = new(SceneNavigationInformation.Instance.NavigationGaps);

            var ownerPosBottomY = _brain.Owner.GetComponent<Collider2D>().bounds.min.y; // Debug.Log("Owner Bottom Y: " + ownerPosBottomY);
            var ownerPosX = _brain.Owner.transform.position.x;
            var brainTargetPosBottomY = _brain.Target.GetComponent<Collider2D>().bounds.min.y; // Debug.Log("Target Bottom Y: " + targetBottomY);
            var brainfTargetPosX = _brain.Target.transform.position.x;

            var isAtSameHeightAsTarget = Mathf.Abs(ownerPosBottomY - brainTargetPosBottomY) < 3f; // Debug.Log("Height difference: " + Mathf.Abs(ownerPosBottomY - brainTargetPosBottomY));
            var isAboveTarget = ownerPosBottomY > brainTargetPosBottomY + 3f;
            var isBelowTarget = ownerPosBottomY < brainTargetPosBottomY - 3f;

            List<WaypointInformation> waypointAtSameHeightAsOwner = new(WaypointsAtSameHeightAsOwner());
            var waypointNearestToOwner = WaypointNearestToOwner_AtSameHeight();

            List<WaypointInformation> waypointsAtSameHeightAsTarget = new(WaypointsAtSameHeightAsTarget());
            var waypointNearestToTarget = WaypointNearestToTarget_AtSameHeight();

            if (isAtSameHeightAsTarget)
            {
                Debug.Log("The target is at the same height as the owner");

                var gapsAtSameHeight = GapsAtSameHeightAsOwner();
                if (gapsAtSameHeight.Count == 0)
                {
                    // Move towards the target
                    _targetToMoveTowards = _brain.Target; //Debug.Log("1");
                    _avoidLadders = true;
                    _pathwayToTargetIsDefined = true; return;
                }

                var hasNavigationalGap = false;
                foreach (var gap in gapsAtSameHeight)
                {
                    var gapPosX = gap.transform.position.x;
                    if (gapPosX > ownerPosX && gapPosX < brainfTargetPosX) // THERE IS A GAP BETWEEN OWNER AND TARGET
                    {
                        hasNavigationalGap = true; Debug.Log("There is a gap between the owner and the target");
                        break;
                    }
                }

                if (hasNavigationalGap)
                {
                    // Figure out the nearest ladder to go up or down
                    _targetToMoveTowards = waypointNearestToOwner.transform;
                    DeterminePathwayToTarget(isAtSameHeightAsTarget); //Debug.Log("2");
                }
                else // if there is no gap between the owner and the target
                {
                    // Move towards the target
                    _targetToMoveTowards = _brain.Target; Debug.Log("Moving towards the target; Target: " + _targetToMoveTowards.name + "Brain Target: " + _brain.Target.name);
                    _avoidLadders = true;
                    _pathwayToTargetIsDefined = true;
                }
                return;
            }

            if (isBelowTarget)
            {
                //Debug.Log("The Owner is below the target. >>ownerPosBottomY: " + ownerPosBottomY + " >>targetBottomY: " + targetBottomY);
                _nextLadderMoveIsUp = true; // Must come before DeterminePathwayToTarget() because of edge case (when target on another tree)
                DeterminePathwayToTarget(); // Sets _targetToMoveTowards
                return;
            }

            if (isAboveTarget)
            {
                //Debug.Log("The Owner is above the target. >>ownerPosBottomY: " + ownerPosBottomY + " >>targetBottomY: " + targetBottomY);
                _nextLadderMoveIsUp = false; // Must come before DeterminePathwayToTarget() because of edge case (when target on another tree)
                DeterminePathwayToTarget(); // Sets _targetToMoveTowards
                return;
            }

            List<WaypointInformation> GapsAtSameHeightAsOwner() // same height as the owner
            {
                List<WaypointInformation> gapsAtSameHeight = new List<WaypointInformation>();
                foreach (var gap in navigationGaps)
                {
                    if (Mathf.Abs(gap.transform.position.y - ownerPosBottomY) < 2f)
                    {
                        gapsAtSameHeight.Add(gap);
                    }
                }
                return gapsAtSameHeight;
            }

            List<WaypointInformation> WaypointsAtSameHeightAsOwner() // same height as the owner
            {
                List<WaypointInformation> waypointsAtSameHeight = new List<WaypointInformation>();
                foreach (var waypoint in navigationWaypoints)
                {
                    if (Mathf.Abs(waypoint.transform.position.y - ownerPosBottomY) < 2f)
                    {
                        waypointsAtSameHeight.Add(waypoint);
                    }
                }
                return waypointsAtSameHeight;
            }

            WaypointInformation WaypointNearestToOwner_AtSameHeight()
            {
                if (waypointAtSameHeightAsOwner.Count == 0) return null;
                Debug.Log("Waypoints at the same height as the owner: " + waypointAtSameHeightAsOwner.Count);
                WaypointInformation waypointNearest = null;

                foreach (var waypoint in waypointAtSameHeightAsOwner)
                {
                    var waypointDistance = Mathf.Abs(waypoint.transform.position.x - ownerPosX);
                    var distanceToLastKnownNearest = Mathf.Abs(waypointAtSameHeightAsOwner[0].transform.position.x - ownerPosX);

                    bool hasGapBetween = false;
                    foreach (var gap in GapsAtSameHeightAsOwner())
                    {
                        var gapPositionX = gap.transform.position.x;
                        if ((gapPositionX > Mathf.Min(waypoint.transform.position.x, ownerPosX))
                        && (gapPositionX < Mathf.Max(waypoint.transform.position.x, ownerPosX))) // GAP IS BETWEEN THE TWO WAYPOINTS
                        {
                            hasGapBetween = true;
                            break;
                        }
                    }

                    if (!hasGapBetween && waypointDistance <= distanceToLastKnownNearest) // NO GAP BETWEEN AND CLOSER
                    {
                        waypointNearest = waypoint;
                    }
                }
                Debug.Log(waypointNearest + " is the nearest waypoint to the owner");
                return waypointNearest;
            }

            List<WaypointInformation> GapsAtSameHeightAsTarget() // same height as the target
            {
                List<WaypointInformation> gapsAtSameHeight = new List<WaypointInformation>();
                foreach (var gap in navigationGaps)
                {
                    if (Mathf.Abs(gap.transform.position.y - brainTargetPosBottomY) < 2f)
                    {
                        gapsAtSameHeight.Add(gap);
                    }
                }
                return gapsAtSameHeight;
            }

            List<WaypointInformation> WaypointsAtSameHeightAsTarget() // same height as the target
            {
                List<WaypointInformation> waypointsAtSameHeight = new List<WaypointInformation>();
                foreach (var waypoint in navigationWaypoints)
                {
                    if (Mathf.Abs(waypoint.transform.position.y - brainTargetPosBottomY) < 2f)
                    {
                        waypointsAtSameHeight.Add(waypoint);
                    }
                }
                return waypointsAtSameHeight;
            }

            WaypointInformation WaypointNearestToTarget_AtSameHeight()
            {
                if (waypointsAtSameHeightAsTarget.Count == 0) return null;

                WaypointInformation waypointNearest = waypointsAtSameHeightAsTarget[0];

                foreach (var waypoint in waypointsAtSameHeightAsTarget)
                {
                    var waypointDistance = Mathf.Abs(waypoint.transform.position.x - brainfTargetPosX);
                    var distanceToLastKnownNearest = Mathf.Abs(waypointNearest.transform.position.x - brainfTargetPosX);

                    bool hasGapBetween = false;
                    foreach (var gap in GapsAtSameHeightAsTarget())
                    {
                        var gapPositionX = gap.transform.position.x;
                        if ((gapPositionX > Mathf.Min(waypoint.transform.position.x, brainfTargetPosX))
                        && (gapPositionX < Mathf.Max(waypoint.transform.position.x, brainfTargetPosX))) // GAP IS BETWEEN THE TWO WAYPOINTS
                        {
                            hasGapBetween = true;
                            break;
                        }
                    }
                    Debug.Log(waypoint.name + " has a distance of " + waypointDistance + " from the target: " + _brain.Target + ". HasGaps: " + hasGapBetween);
                    if (!hasGapBetween && waypointDistance < distanceToLastKnownNearest) // NO GAP BETWEEN AND CLOSER
                    {
                        waypointNearest = waypoint;
                    }
                }
                Debug.Log(waypointNearest.name + " is the nearest waypoint to the target");
                return waypointNearest;
            }

            WaypointInformation AdjacentWaypointAtBrainTargetHeight(WaypointInformation waypointVerified)
            {
                WaypointInformation adjacentWaypoint = null;
                foreach (var waypoint in navigationWaypoints)
                {
                    //Debug.Log(Mathf.Abs(waypoint.transform.position.x - targetWaypoint.transform.position.x) + " is the difference between " + waypoint.name + " and the target"); //
                    //Debug.Log(Mathf.Abs(waypoint.transform.position.y - targetBottomY) + " is the difference between " + waypoint.name + " and the BRAIN_target"); //

                    if (Mathf.Abs(waypoint.transform.position.x - waypointVerified.transform.position.x) > 2) continue; // NOT ON THE SAME VERTICAL LINE
                    if (Mathf.Abs(waypoint.transform.position.y - brainTargetPosBottomY) < 2f) // SAME HEIGHT AS TARGET BUILDING
                    {
                        adjacentWaypoint = waypoint;
                        break;
                    }
                }
                Debug.Log(adjacentWaypoint.name + " is the adjacent waypoint to the target");
                return adjacentWaypoint;
            }

            WaypointInformation[] AdjacentWaypointsDirectlyAbove(WaypointInformation waypointVerified)
            {
                List<WaypointInformation> adjacentWaypoints = new List<WaypointInformation>();
                foreach (var waypoint in navigationWaypoints)
                {
                    if (Mathf.Abs(waypoint.transform.position.x - waypointVerified.transform.position.x) > 2) continue; // NOT ON THE SAME VERTICAL LINE
                    if (waypoint.transform.position.y > waypointVerified.transform.position.y) // ABOVE THE TARGET
                    {
                        adjacentWaypoints.Add(waypoint);
                    }
                }
                return adjacentWaypoints.ToArray();
            }

            WaypointInformation[] AdjacentWaypointsDirectlyBelow(WaypointInformation waypointVerified)
            {
                List<WaypointInformation> adjacentWaypoints = new List<WaypointInformation>();
                foreach (var waypoint in navigationWaypoints)
                {
                    if (Mathf.Abs(waypoint.transform.position.x - waypointVerified.transform.position.x) > 2) continue; // NOT ON THE SAME VERTICAL LINE
                    if (waypoint.transform.position.y < waypointVerified.transform.position.y) // BELOW THE TARGET
                    {
                        adjacentWaypoints.Add(waypoint);
                    }
                }
                return adjacentWaypoints.ToArray();
            }

            bool HasGapsBetweenWaypointAndTarget(WaypointInformation waypoint)
            {
                var waypointPosX = waypoint.transform.position.x;
                var targetPosX = _brain.Target.transform.position.x;

                foreach (var gap in GapsAtSameHeightAsTarget())
                {
                    var gapPosX = gap.transform.position.x;
                    if ((gapPosX > Mathf.Min(waypointPosX, targetPosX))
                    && (gapPosX < Mathf.Max(waypointPosX, targetPosX))) // GAP IS BETWEEN THE WAYPOINT AND TARGET
                    {
                        return true;
                    }
                }
                return false;
            }

            bool HasGapBetweenTwoPoints(WaypointInformation point1, WaypointInformation point2)
            {
                var point1PosX = point1.transform.position.x;
                var point2PosX = point2.transform.position.x;

                List<WaypointInformation> gapsAtSameHeight = GetGapsAtSameHeight(point1, navigationGaps);
                if (gapsAtSameHeight.Count == 0) return false; // NO GAPS AT THE SAME HEIGHT

                foreach (var gap in gapsAtSameHeight)
                {
                    var gapPosX = gap.transform.position.x;
                    if ((gapPosX > Mathf.Min(point1PosX, point2PosX))
                    && (gapPosX < Mathf.Max(point1PosX, point2PosX))) // GAP IS BETWEEN THE TWO POINTS
                    {
                        return true;
                    }
                }
                return false; // NO GAPS BETWEEN THE TWO POINTS

                List<WaypointInformation> GetGapsAtSameHeight(WaypointInformation point1, List<WaypointInformation> navigationGaps)
                {
                    List<WaypointInformation> gapsAtSameHeight = new List<WaypointInformation>();
                    foreach (var gap in navigationGaps)
                    {
                        if (Mathf.Abs(gap.transform.position.y - point1.transform.position.y) < 2f)
                        {
                            gapsAtSameHeight.Add(gap);
                        }
                    }

                    return gapsAtSameHeight;
                }
            }

            void DeterminePathwayToTarget(bool isAtSameHeightAsTarget = false)
            {
                if (isAtSameHeightAsTarget)
                {
                    // For now, just try to go down   !!!! NEEDS IMPROVING
                    _nextLadderMoveIsUp = false;
                    _avoidLadders = false;
                    _pathwayToTargetIsDefined = true;
                    Debug.LogError("NEED BETTER IMPLEMENTATION: This is a temporary solution");
                }
                else // if it is not at the same height as the target
                {   // Use WaypointAtSameHeightAsOwner() instead of waypointAtSameHeightAsOwner because waypointAtSameHeightAsOwner changes
                    _avoidLadders = false;
                    _pathwayToTargetIsDefined = true;

                    for (int i = 0; i < WaypointsAtSameHeightAsOwner().Count; i++)
                    {
                        // Figure out the nearest ladder to go up or down
                        var validWaypointNearestToOwner = WaypointNearestToOwner_AtSameHeight();
                        if (validWaypointNearestToOwner == null) break;

                        // Find waypoints immediatly above or below that ladder
                        var targetWaypointAdjacentToOwner = AdjacentWaypointAtBrainTargetHeight(validWaypointNearestToOwner);
                        Debug.Log("Valid Waypoint Nearest To Owner: " + validWaypointNearestToOwner.name + " Target Adjacent To Owner: " + targetWaypointAdjacentToOwner.name);
                        if (targetWaypointAdjacentToOwner == null)
                        {
                            // This waypoint does not take us to the _brain.Target. Remove it from the list
                            waypointAtSameHeightAsOwner.Remove(validWaypointNearestToOwner);
                            continue;
                        }
                        // else if there IS a waypoint above or bellow the target waypoint
                        if (HasGapsBetweenWaypointAndTarget(targetWaypointAdjacentToOwner))
                        {
                            // This waypoint does not take us to the _brain.Target. Remove it from the list
                            waypointAtSameHeightAsOwner.Remove(validWaypointNearestToOwner);
                            continue;
                        }
                        // There is a path to the target without any gaps.

                        Debug.Log("There is a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " and " + targetWaypointAdjacentToOwner.name);

                        _targetToMoveTowards = validWaypointNearestToOwner.transform; // SET THE TARGET TO MOVE TOWARDS ---------------------------------------
                        _adjacentTargetWaypoint = targetWaypointAdjacentToOwner.transform;
                    }

                    if (_targetToMoveTowards != null) return; // WE FOUND A PATH TO THE TARGET WITHOUT ANY GAPS ----------------------------------------------

                    for (int i = 0; i < WaypointsAtSameHeightAsOwner().Count; i++)
                    {
                        // Figure out the nearest ladder to go up or down
                        var validWaypointNearestToOwner = WaypointNearestToOwner_AtSameHeight();
                        if (validWaypointNearestToOwner == null) break;

                        // Find waypoints immediatly above or below that ladder
                        var targetWaypointAdjacentToTarget = AdjacentWaypointAtBrainTargetHeight(validWaypointNearestToOwner);

                        if (targetWaypointAdjacentToTarget == null)
                        {
                            // This waypoint does not take us to the _brain.Target. Remove it from the list
                            waypointAtSameHeightAsOwner.Remove(validWaypointNearestToOwner);
                            continue;
                        }
                        // else if there IS a waypoint above or bellow the target waypoint
                        if (HasGapsBetweenWaypointAndTarget(targetWaypointAdjacentToTarget))
                        {
                            // This waypoint does not take us to the _brain.Target. Remove it from the list
                            waypointAtSameHeightAsOwner.Remove(validWaypointNearestToOwner);
                            continue;
                        }
                        // There is a path to the target without any gaps.

                        Debug.Log("There is a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " and " + targetWaypointAdjacentToTarget.name);

                        _targetToMoveTowards = validWaypointNearestToOwner.transform; // SET THE TARGET TO MOVE TOWARDS ---------------------------------------
                        _adjacentTargetWaypoint = targetWaypointAdjacentToTarget.transform;
                    }

                    if (_targetToMoveTowards != null) return; // WE FOUND A PATH TO THE TARGET WITHOUT ANY GAPS ----------------------------------------------

                    // Continuing means we have not found a path immediately at the height of the target
                    // Lets try to find a path to the target without any gaps. Looking above and below the owner
                    var potentiaTargetsToMoveTowards = new List<Transform>();
                    var potentiaAdjacentWaypoints = new List<Transform>();
                    // It is important to first renew waypointAtSameHeightAsOwner because it has been modified on the logica above
                    waypointAtSameHeightAsOwner = new(WaypointsAtSameHeightAsOwner());

                    var validWaypointNearestToTarget = WaypointNearestToTarget_AtSameHeight();

                    for (int i = 0; i < WaypointsAtSameHeightAsOwner().Count; i++)
                    {
                        // Figure out the nearest ladder to go up or down
                        var validWaypointNearestToOwner = WaypointNearestToOwner_AtSameHeight(); //Debug.Log("Valid Waypoint Nearest To Owner: " + validWaypointNearestToOwner.name);
                        // Find waypoints above of that ladder 
                        var targetWaypointsAboveNavOrigin = AdjacentWaypointsDirectlyAbove(validWaypointNearestToOwner);
                        var targetWaypointsBelowNavOrigin = AdjacentWaypointsDirectlyBelow(validWaypointNearestToOwner);

                        foreach (var originPoint in targetWaypointsAboveNavOrigin)
                        {
                            Debug.Log("1 - Origin: " + originPoint.name + " Target: " + validWaypointNearestToTarget.name);
                            var adjacentWaypointsAboveTarget = AdjacentWaypointsDirectlyAbove(validWaypointNearestToTarget);
                            Debug.Log("1 - Adjacent Waypoints Above Target: " + adjacentWaypointsAboveTarget.Length);
                            foreach (var targetPoint in adjacentWaypointsAboveTarget)
                            {
                                if (originPoint.transform.position.y != targetPoint.transform.position.y) continue;
                                // We have found a waypoint at the same height as the target.
                                Debug.Log("1a - Origin: " + originPoint.name + " Target: " + targetPoint.name);
                                if (!HasGapBetweenTwoPoints(originPoint, targetPoint))
                                {
                                    Debug.Log("1a - Found a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " and " + targetPoint.name);
                                    potentiaTargetsToMoveTowards.Add(validWaypointNearestToOwner.transform);
                                    potentiaAdjacentWaypoints.Add(originPoint.transform);
                                }
                            }

                            foreach (var targetPoint in AdjacentWaypointsDirectlyBelow(validWaypointNearestToTarget))
                            {
                                if (originPoint.transform.position.y != targetPoint.transform.position.y) continue;
                                // We have found a waypoint at the same height as the target.
                                Debug.Log("1b - Origin: " + originPoint.name + " Target: " + targetPoint.name);
                                if (!HasGapBetweenTwoPoints(originPoint, targetPoint))
                                {
                                    Debug.Log("1b - Found a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " and " + targetPoint.name);
                                    potentiaTargetsToMoveTowards.Add(validWaypointNearestToOwner.transform);
                                    potentiaAdjacentWaypoints.Add(originPoint.transform);
                                }
                            }
                        }

                        foreach (var originPoint in targetWaypointsBelowNavOrigin)
                        {
                            Debug.Log("2 - Origin: " + originPoint.name + " Target: " + validWaypointNearestToTarget.name);
                            var adjacentWaypointsBelowTarget = AdjacentWaypointsDirectlyBelow(validWaypointNearestToTarget);
                            Debug.Log("2 - Adjacent Waypoints Below Target: " + adjacentWaypointsBelowTarget.Length);
                            foreach (var targetPoint in adjacentWaypointsBelowTarget)
                            {
                                Debug.Log("2a - Origin: " + originPoint.name + " Target: " + targetPoint.name);
                                Debug.Log("2a - Origin: " + originPoint.transform.position.y + " Target: " + targetPoint.transform.position.y);
                                if (originPoint.transform.position.y != targetPoint.transform.position.y) continue;

                                // We have found a waypoint at the same height as the target.
                                Debug.Log("CHecking for gaps between " + originPoint.name + " and " + targetPoint.name);
                                Debug.Log("Gaps?" + HasGapBetweenTwoPoints(originPoint, targetPoint));
                                if (!HasGapBetweenTwoPoints(originPoint, targetPoint))
                                {
                                    Debug.Log("2a - Found a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " via " + originPoint + " and eventually" + targetPoint.name);
                                    potentiaTargetsToMoveTowards.Add(validWaypointNearestToOwner.transform);
                                    potentiaAdjacentWaypoints.Add(originPoint.transform);
                                }
                            }

                            foreach (var targetPoint in AdjacentWaypointsDirectlyAbove(validWaypointNearestToTarget))
                            {
                                Debug.Log("2b - Origin: " + originPoint.name + " Target: " + targetPoint.name);
                                Debug.Log("2a - Origin: " + originPoint.transform.position.y + " Target: " + targetPoint.transform.position.y);
                                if (originPoint.transform.position.y != targetPoint.transform.position.y) continue;

                                // We have found a waypoint at the same height as the target.
                                if (!HasGapBetweenTwoPoints(originPoint, targetPoint))
                                {
                                    Debug.Log("2b - Found a path to the target without any gaps, it is: " + validWaypointNearestToOwner.name + " and " + targetPoint.name);
                                    potentiaTargetsToMoveTowards.Add(validWaypointNearestToOwner.transform);
                                    potentiaAdjacentWaypoints.Add(originPoint.transform);
                                }
                            }
                        }

                        if (potentiaTargetsToMoveTowards.Count < 1) { Debug.LogError("UNEXPECTED: No valid path found"); return; }

                        // Finally we choose a random valid path
                        var randomIndex = Random.Range(0, potentiaTargetsToMoveTowards.Count);
                        _targetToMoveTowards = potentiaTargetsToMoveTowards[randomIndex];
                        _adjacentTargetWaypoint = potentiaAdjacentWaypoints[randomIndex];

                        Debug.Log("Adjacent Target Waypoint: " + _adjacentTargetWaypoint.transform.position.y + " Target: " + _targetToMoveTowards.transform.position.y);
                        if (_adjacentTargetWaypoint.transform.position.y > _targetToMoveTowards.transform.position.y)
                        {
                            _nextLadderMoveIsUp = true;
                        }
                        else _nextLadderMoveIsUp = false;
                    }
                }
            }
        }

        protected virtual void MoveTowardsNextPathLocation()
        {
            var ownerPosX = _brain.Owner.transform.position.x;
            var targetPosX = _targetToMoveTowards.transform.position.x;

            if (_targetToMoveTowards == null)
            {
                Debug.LogError("UNEXPECTED: No valid waypoint found");
                _characterHorizontalMovement.SetHorizontalMove(0f);
                return;
            }

            if (Mathf.Abs(ownerPosX - targetPosX) < MinimumDistance)
            {
                _characterHorizontalMovement.SetHorizontalMove(0f);
                return;
            }

            if (ownerPosX < targetPosX)
            {
                _characterHorizontalMovement.SetHorizontalMove(1f);
            }
            else
            {
                _characterHorizontalMovement.SetHorizontalMove(-1f);
            }
        }

        IEnumerator AvoidLaddersTemporarily()
        {
            _tempAvoidLadders = true;
            yield return new WaitForSeconds(1f);
            _tempAvoidLadders = false;
        }

        protected virtual void AddDetectTargetLadder_Script()
        {
            _detectTargetLadder = transform.parent.GetComponentInChildren<AIDecision_DetectTargetLadder>();
            if (_detectTargetLadder == null)
            {
                _detectTargetLadder = gameObject.AddComponent<AIDecision_DetectTargetLadder>();
            }
        }
    }
}
