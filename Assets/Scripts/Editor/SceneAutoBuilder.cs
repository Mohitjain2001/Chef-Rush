using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using YesChef.Utils;

namespace YesChef.EditorTools
{
    [InitializeOnLoad]
    public class SceneAutoBuilder
    {
        static SceneAutoBuilder()
        {
            EditorApplication.delayCall += OnEditorLoaded;
        }

        private static void OnEditorLoaded()
        {
            // Auto build scene if active scene is empty or missing environment
            if (GameObject.Find("KitchenEnvironment") == null)
            {
                BuildScene();
            }
        }

        [MenuItem("YesChef/Build & Setup Kitchen Scene")]
        public static void BuildScene()
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();

            GameObject setupObj = new GameObject("TempSceneSetup");
            KitchenSceneSetup setup = setupObj.AddComponent<KitchenSceneSetup>();
            setup.BuildScene();
            Object.DestroyImmediate(setupObj);

            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            Debug.Log("[SceneAutoBuilder] YesChef Kitchen Scene created and saved to active scene.");
        }
    }
}
