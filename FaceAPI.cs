using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

using Windows.Storage;

namespace SmartDevice
{
    public static class FaceAPI
    {
        // Cognitive Services Face API client
        private static FaceServiceClient faceServiceClient = new FaceServiceClient(AzureCredentials.subscriptionKey);

        public static async Task<string> GetViewerName(StorageFile photoFile)
        {
            // generic placeholder for viewer's name
            string personName = "friend";

            using (FileStream photoStream = new FileStream(photoFile.Path, FileMode.Open))
            {
                // ask Face API to detect faces in photo 
                Face[] faces = await faceServiceClient.DetectAsync(photoStream);

                if (faces.Length > 0)
                {
                    // convert result to array of id's to identify
                    Guid[] faceIds = faces.Select(face => face.FaceId).ToArray();

                    // ask face API to identify face from a trained person group and the id's we got back
                    IdentifyResult[] results = await faceServiceClient.IdentifyAsync(AzureCredentials.personGroup, faceIds);

                    // go through the returned identification results from the Face API
                    foreach (IdentifyResult identifyResult in results)
                    {
                        // if we have a prediction for who they are
                        if (identifyResult.Candidates.Length > 0)
                        {
                            Guid candidateId = identifyResult.Candidates[0].PersonId;
                            // extract the candidate's name using their ID
                            Person person = await faceServiceClient.GetPersonAsync(AzureCredentials.personGroup, candidateId);
                            personName = person.Name;
                        }
                    }
                }
                return personName;
            }
        }
    }
}
