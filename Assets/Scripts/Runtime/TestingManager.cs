using System.Collections.Generic;
using System.IO;
using DavidJalbert;
using MobX.Utilities.Inspector;
using Newtonsoft.Json;
using UnityEngine;

namespace Default
{
    public class TestingManager : MonoBehaviour
    {
        public bool useEvaluationFile;

        [ConditionalHide("useEvaluationFile", true), TextArea] public string testModelData;
        [ConditionalHide("useEvaluationFile", false), TextArea, Tooltip("Takes an eval file from the Evaluation folder!")] public string evaluationFileName;

        [Space]
        [Header("Testing")]
        [SerializeField] private TinyCarCamera carCamera;
        [SerializeField] private AICarController testingCar;
        [SerializeField] private int testModelIndex;
        [SerializeField] private int testTrackIndex;
        [SerializeField] private List<RaceTrack> raceTracks;


        public async void TestModel()
        {
            carCamera.whatToFollow = testingCar.transform;

            string carLabel;
            SerializedNetworkData networkData;
            int testTrackIndexClamped = Mathf.Clamp(testTrackIndex, 0, raceTracks.Count - 1);

            if (useEvaluationFile)
            {
                var path = Path.Combine(Application.dataPath, "Evaluation/", evaluationFileName);
                var jsonContent = File.ReadAllText(path);

                var data = JsonConvert.DeserializeObject<EvolutionEvaluationData>(jsonContent);
                var modelIndexClamped = Mathf.Clamp(testModelIndex, 0, data.Evaluations.Count);

                networkData = data.Evaluations[modelIndexClamped].fittestNetwork;
                carLabel = "";
                // @$"{string.Join(", ", data.Evaluations[modelIndexClamped].LayerStructure)}, 
                // Fitness: {data.Evaluations[modelIndexClamped].fittestNetwork.fitness}";
            }
            else
            {
                networkData = JsonConvert.DeserializeObject<SerializedNetworkData>(testModelData);
                carLabel = "";
                // $"{string.Join(", ", networkData.layers)}_Fitness: {networkData.fitness}";
            }

            raceTracks[testTrackIndexClamped].spawn.GetPositionAndRotation(out var spawnPos, out var spawnRot);

            await testingCar.SetPositionAndRotation(spawnPos, spawnRot);
            await testingCar.SetTarget(raceTracks[testTrackIndexClamped].targets[0].transform.position);

            testingCar.SetInputBuffer(networkData.layers[0] >= 8);
            testingCar.SetAI(new NeuralNetwork(networkData));
            testingCar.SetUIHeader(carLabel);
            testingCar.EnableDrivingAI(true);
        }
    }
}