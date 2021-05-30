using Cinemachine;
using MoreMountains.CorgiEngine;
using System;
using System.Linq;
using UnityEngine;

public class LevelManagerCinemachineBased : LevelManager
{
    protected static new LevelManagerCinemachineBased _instance;

    public static new LevelManagerCinemachineBased Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManagerCinemachineBased>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<LevelManagerCinemachineBased>();
                }
            }
            return _instance;
        }
    }


    protected override void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        _instance = this;

        InstantiatePlayableCharacters();
    }

    public CinemachineVirtualCamera[] vCams;
    /// 
    /// Gets current camera, points number, start time, etc.
    /// 
    protected override void Initialization()
    {
        // storage
        //LevelCameraController = FindObjectOfType();
        for (int i = 0; i < vCams.Length; i++)
        {
            vCams[i].Follow = Players[0].transform;
        }
        _savedPoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;

        // we store all the checkpoints present in the level, ordered by their x value
        if ((CheckpointAttributionAxis == CheckpointsAxis.x) && (CheckpointAttributionDirection == CheckpointDirections.Ascending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(o => o.transform.position.x).ToList();
        }
        if ((CheckpointAttributionAxis == CheckpointsAxis.x) && (CheckpointAttributionDirection == CheckpointDirections.Descending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderByDescending(o => o.transform.position.x).ToList();
        }
        if ((CheckpointAttributionAxis == CheckpointsAxis.y) && (CheckpointAttributionDirection == CheckpointDirections.Ascending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(o => o.transform.position.y).ToList();
        }
        if ((CheckpointAttributionAxis == CheckpointsAxis.y) && (CheckpointAttributionDirection == CheckpointDirections.Descending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderByDescending(o => o.transform.position.y).ToList();
        }
        if ((CheckpointAttributionAxis == CheckpointsAxis.z) && (CheckpointAttributionDirection == CheckpointDirections.Ascending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(o => o.transform.position.z).ToList();
        }
        if ((CheckpointAttributionAxis == CheckpointsAxis.z) && (CheckpointAttributionDirection == CheckpointDirections.Descending))
        {
            Checkpoints = FindObjectsOfType<CheckPoint>().OrderByDescending(o => o.transform.position.z).ToList();
        }

        // we assign the first checkpoint 
        CurrentCheckPoint = Checkpoints.Count > 0 ? Checkpoints[0] : null;
    }

    public void SetActiveCamera(int index)
    {
        vCams[index].gameObject.SetActive(true);
    }
}
