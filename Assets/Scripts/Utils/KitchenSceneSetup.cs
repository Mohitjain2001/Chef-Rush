using UnityEngine;
using UnityEngine.UI;
using YesChef.Data;
using YesChef.Items;
using YesChef.Managers;
using YesChef.Player;
using YesChef.Stations;
using YesChef.UI;

namespace YesChef.Utils
{
    public class KitchenSceneSetup : MonoBehaviour
    {
        [Header("Auto Setup Config")]
        public bool runOnAwake = true;

        private void Awake()
        {
            if (runOnAwake)
            {
                BuildScene();
            }
        }

        [ContextMenu("Build Scene Layout")]
        public void BuildScene()
        {
            // Clear existing generated setup if present
            GameObject existing = GameObject.Find("KitchenEnvironment");
            if (existing != null) DestroyImmediate(existing);

            GameObject kitchenEnv = new GameObject("KitchenEnvironment");

            // 1. Camera & Lighting
            SetupCameraAndLighting(kitchenEnv);

            // 2. Kitchen Floor & 4 Enclosing Walls
            SetupRoomGeometry(kitchenEnv);

            // 3. Game & Order Managers
            GameObject managersObj = new GameObject("Managers");
            managersObj.transform.SetParent(kitchenEnv.transform);
            GameManager gameMgr = managersObj.AddComponent<GameManager>();
            OrderManager orderMgr = managersObj.AddComponent<OrderManager>();

            // Create Ingredient SOs
            IngredientSO vegSO = CreateIngredientSO(IngredientType.Vegetable, "Vegetable", 20, PrepStationType.CuttingTable, 2.0f, new Color(0.1f, 0.85f, 0.25f), new Color(0.4f, 0.95f, 0.4f), PrimitiveType.Sphere, PrimitiveType.Cube, new Vector3(0.45f, 0.45f, 0.45f), new Vector3(0.45f, 0.18f, 0.45f));
            IngredientSO cheeseSO = CreateIngredientSO(IngredientType.Cheese, "Cheese", 10, PrepStationType.None, 0.0f, new Color(1.0f, 0.85f, 0.1f), new Color(1.0f, 0.85f, 0.1f), PrimitiveType.Cube, PrimitiveType.Cube, new Vector3(0.45f, 0.35f, 0.45f), new Vector3(0.45f, 0.35f, 0.45f));
            IngredientSO meatSO = CreateIngredientSO(IngredientType.Meat, "Meat", 30, PrepStationType.Stove, 6.0f, new Color(0.9f, 0.25f, 0.25f), new Color(0.4f, 0.2f, 0.12f), PrimitiveType.Cube, PrimitiveType.Cube, new Vector3(0.5f, 0.25f, 0.5f), new Vector3(0.5f, 0.25f, 0.5f));

            orderMgr.vegetableSO = vegSO;
            orderMgr.cheeseSO = cheeseSO;
            orderMgr.meatSO = meatSO;

            // 4. Stations according to diagram
            // Left Wall: 4 Customer Windows
            CustomerWindowStation[] windows = new CustomerWindowStation[4];
            float[] windowZPositions = new float[] { 4.5f, 1.5f, -1.5f, -4.5f };
            for (int i = 0; i < 4; i++)
            {
                windows[i] = CreateCustomerWindow(kitchenEnv.transform, i, new Vector3(-7.5f, 0.5f, windowZPositions[i]));
            }
            orderMgr.customerWindows = windows;

            // Refrigerator at Top Right (x=6.5, z=4.0)
            CreateFridgeStationGroup(kitchenEnv.transform, new Vector3(6.5f, 0.5f, 4.0f), vegSO, cheeseSO, meatSO);

            // Table (Cutting Table) at Center-Right (x=1.0, z=2.0)
            CreateCuttingTable(kitchenEnv.transform, new Vector3(1.0f, 0.5f, 2.0f));

            // Stove at Center-Right (x=1.0, z=-2.0)
            CreateStove(kitchenEnv.transform, new Vector3(1.0f, 0.5f, -2.0f));

            // Trash at Bottom Right (x=6.5, z=-4.0)
            CreateTrash(kitchenEnv.transform, new Vector3(6.5f, 0.5f, -4.0f));

            // 5. Player Character (x=-2.0, z=0.0)
            SetupPlayer(kitchenEnv.transform, new Vector3(-2.0f, 0.5f, 0.0f));

            // 6. UI Canvas & Overlay Elements
            SetupUI(kitchenEnv.transform, gameMgr);

            Debug.Log("[KitchenSceneSetup] YES CHEF! Kitchen Scene generated with clear 3D station labels.");
        }

        private IngredientSO CreateIngredientSO(IngredientType type, string name, int score, PrepStationType station, float prepTime, Color rawCol, Color prepCol, PrimitiveType rawShape, PrimitiveType prepShape, Vector3 rawScale, Vector3 prepScale)
        {
            IngredientSO so = ScriptableObject.CreateInstance<IngredientSO>();
            so.ingredientType = type;
            so.ingredientName = name;
            so.scoreValue = score;
            so.requiredStation = station;
            so.prepTime = prepTime;
            so.rawColor = rawCol;
            so.preparedColor = prepCol;
            so.rawPrimitiveShape = rawShape;
            so.preparedPrimitiveShape = prepShape;
            so.rawScale = rawScale;
            so.preparedScale = prepScale;
            return so;
        }

        private void SetupCameraAndLighting(GameObject parent)
        {
            GameObject camObj = GameObject.FindWithTag("MainCamera");
            if (camObj == null)
            {
                camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            camObj.transform.SetParent(parent.transform);

            camObj.transform.position = new Vector3(0f, 15.0f, -0.5f);
            camObj.transform.rotation = Quaternion.Euler(85f, 0f, 0f);

            Camera cam = camObj.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 7.5f;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);

            GameObject lightObj = new GameObject("Directional Light");
            lightObj.transform.SetParent(parent.transform);
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 1.25f;
            light.color = new Color(1.0f, 0.96f, 0.92f);
        }

        private void SetupRoomGeometry(GameObject parent)
        {
            GameObject room = new GameObject("RoomGeometry");
            room.transform.SetParent(parent.transform);

            // Floor (16x14) - Dark clean floor tiles for high contrast
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(room.transform);
            floor.transform.position = new Vector3(0f, -0.25f, 0f);
            floor.transform.localScale = new Vector3(16f, 0.5f, 14f);
            SetMaterialColor(floor, new Color(0.24f, 0.27f, 0.3f));

            // Walls
            Color wallColor = new Color(0.35f, 0.4f, 0.45f);
            CreateWall(room.transform, "Wall_Top", new Vector3(0f, 1.0f, 7.0f), new Vector3(16f, 2.5f, 0.5f), wallColor);
            CreateWall(room.transform, "Wall_Bottom", new Vector3(0f, 1.0f, -7.0f), new Vector3(16f, 2.5f, 0.5f), wallColor);
            CreateWall(room.transform, "Wall_Left", new Vector3(-8.0f, 1.0f, 0f), new Vector3(0.5f, 2.5f, 14f), wallColor);
            CreateWall(room.transform, "Wall_Right", new Vector3(8.0f, 1.0f, 0f), new Vector3(0.5f, 2.5f, 14f), wallColor);
        }

        private void CreateWall(Transform parent, string name, Vector3 pos, Vector3 scale, Color col)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            SetMaterialColor(wall, col);
        }

        private CustomerWindowStation CreateCustomerWindow(Transform parent, int index, Vector3 position)
        {
            GameObject winObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            winObj.name = $"CustomerWindow_{index + 1}";
            winObj.transform.SetParent(parent);
            winObj.transform.position = position;
            winObj.transform.localScale = new Vector3(0.9f, 1.2f, 2.2f);
            SetMaterialColor(winObj, new Color(0.85f, 0.25f, 0.25f)); // Red counter

            CustomerWindowStation windowStation = winObj.AddComponent<CustomerWindowStation>();
            windowStation.stationName = $"Customer Window {index + 1}";
            windowStation.windowIndex = index;

            // Create WorldSpace UI Canvas for window facing camera
            GameObject uiCanvasObj = new GameObject("WindowCanvas");
            uiCanvasObj.transform.SetParent(winObj.transform);
            uiCanvasObj.transform.localPosition = new Vector3(0.9f, 1.8f, 0f);
            uiCanvasObj.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            Canvas canvas = uiCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            uiCanvasObj.transform.localScale = Vector3.one * 0.009f;
            RectTransform canvasRT = canvas.GetComponent<RectTransform>();
            canvasRT.sizeDelta = new Vector2(260, 200);

            GameObject panelObj = new GameObject("Panel");
            panelObj.transform.SetParent(uiCanvasObj.transform, false);
            Image panelBg = panelObj.AddComponent<Image>();
            panelBg.color = new Color(0.06f, 0.08f, 0.14f, 0.95f);
            RectTransform panelRT = panelObj.GetComponent<RectTransform>();
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.sizeDelta = Vector2.zero;

            Text titleTxt = CreateText(panelObj.transform, "TitleText", $"ORDER #{index + 1}", 24, FontStyle.Bold, new Color(1.0f, 0.85f, 0.2f), new Vector2(0f, 70f), new Vector2(240, 36));
            Text timerTxt = CreateText(panelObj.transform, "TimerText", "Wait Time: 0s", 18, FontStyle.Bold, Color.cyan, new Vector2(0f, 32f), new Vector2(240, 28));
            Text reqTxt = CreateText(panelObj.transform, "IngredientsText", "Orders...", 18, FontStyle.Bold, Color.white, new Vector2(0f, -30f), new Vector2(240, 80));

            CustomerWindowUI windowUI = winObj.AddComponent<CustomerWindowUI>();
            windowUI.orderTitleText = titleTxt;
            windowUI.timerText = timerTxt;
            windowUI.ingredientsText = reqTxt;
            windowUI.containerPanel = panelObj;
            windowUI.popupAnchor = winObj.transform;

            windowStation.windowUI = windowUI;

            return windowStation;
        }

        private void CreateFridgeStationGroup(Transform parent, Vector3 basePos, IngredientSO veg, IngredientSO cheese, IngredientSO meat)
        {
            GameObject fridgeGroup = new GameObject("RefrigeratorGroup");
            fridgeGroup.transform.SetParent(parent);

            CreateFridgeCounter(fridgeGroup.transform, "Fridge_Veg", basePos + new Vector3(0f, 0f, 1.6f), new Color(0.12f, 0.55f, 0.25f), veg, "VEGETABLE FRIDGE", new Color(0.3f, 0.95f, 0.4f));
            CreateFridgeCounter(fridgeGroup.transform, "Fridge_Cheese", basePos, new Color(0.85f, 0.7f, 0.1f), cheese, "CHEESE FRIDGE", new Color(1.0f, 0.9f, 0.2f));
            CreateFridgeCounter(fridgeGroup.transform, "Fridge_Meat", basePos + new Vector3(0f, 0f, -1.6f), new Color(0.75f, 0.2f, 0.2f), meat, "MEAT FRIDGE", new Color(1.0f, 0.35f, 0.35f));
        }

        private void CreateFridgeCounter(Transform parent, string name, Vector3 pos, Color color, IngredientSO ingredient, string labelText, Color textColor)
        {
            GameObject fridgeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fridgeObj.name = name;
            fridgeObj.transform.SetParent(parent);
            fridgeObj.transform.position = pos;
            fridgeObj.transform.localScale = new Vector3(1.6f, 1.5f, 1.3f);
            SetMaterialColor(fridgeObj, color);

            // Icon label shape on top
            GameObject labelObj = GameObject.CreatePrimitive(ingredient.rawPrimitiveShape);
            labelObj.name = "LabelItem";
            labelObj.transform.SetParent(fridgeObj.transform);
            labelObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            labelObj.transform.localScale = ingredient.rawScale * 0.9f;
            SetMaterialColor(labelObj, ingredient.rawColor);
            DestroyImmediate(labelObj.GetComponent<Collider>());

            FridgeStation fridgeStation = fridgeObj.AddComponent<FridgeStation>();
            fridgeStation.stationName = labelText;
            fridgeStation.ingredientToDispense = ingredient;

            // Big 3D World Label
            CreateWorldLabel(fridgeObj.transform, new Vector3(-0.6f, 1.4f, 0f), labelText, textColor, new Color(0.05f, 0.08f, 0.14f, 0.92f), new Vector2(230, 50));
        }

        private void CreateCuttingTable(Transform parent, Vector3 pos)
        {
            GameObject tableObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tableObj.name = "CuttingTable";
            tableObj.transform.SetParent(parent);
            tableObj.transform.position = pos;
            tableObj.transform.localScale = new Vector3(2.4f, 1.0f, 1.6f);
            SetMaterialColor(tableObj, new Color(0.18f, 0.65f, 0.35f));

            CuttingTableStation station = tableObj.AddComponent<CuttingTableStation>();
            station.stationName = "Cutting Table (Chop Veg)";

            ProgressBarUI progressBar = CreateProgressBar(tableObj.transform, new Vector3(0f, 1.8f, 0f));
            station.progressBar = progressBar;
            progressBar.Show(false);

            // World Label
            CreateWorldLabel(tableObj.transform, new Vector3(0f, 1.3f, 0f), "CUTTING TABLE\n(Chop Veg - 2s)", new Color(0.3f, 0.95f, 0.4f), new Color(0.05f, 0.08f, 0.14f, 0.92f), new Vector2(240, 60));
        }

        private void CreateStove(Transform parent, Vector3 pos)
        {
            GameObject stoveObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stoveObj.name = "Stove";
            stoveObj.transform.SetParent(parent);
            stoveObj.transform.position = pos;
            stoveObj.transform.localScale = new Vector3(2.4f, 1.0f, 1.9f);
            SetMaterialColor(stoveObj, new Color(0.28f, 0.32f, 0.38f)); // Dark metal stove color

            // 2 Burner visual plates (dark circles/cylinders)
            CreateStoveBurner(stoveObj.transform, new Vector3(-0.5f, 0.52f, 0f));
            CreateStoveBurner(stoveObj.transform, new Vector3(0.5f, 0.52f, 0f));

            StoveStation station = stoveObj.AddComponent<StoveStation>();
            station.stationName = "Stove (Cook Meat - 2 Slots)";

            GameObject hp1 = new GameObject("Slot1_HoldPoint");
            hp1.transform.SetParent(stoveObj.transform);
            hp1.transform.localPosition = new Vector3(-0.5f, 0.65f, 0f);

            GameObject hp2 = new GameObject("Slot2_HoldPoint");
            hp2.transform.SetParent(stoveObj.transform);
            hp2.transform.localPosition = new Vector3(0.5f, 0.65f, 0f);

            station.slot1HoldPoint = hp1.transform;
            station.slot2HoldPoint = hp2.transform;

            ProgressBarUI pb1 = CreateProgressBar(hp1.transform, new Vector3(0f, 0.9f, 0f));
            ProgressBarUI pb2 = CreateProgressBar(hp2.transform, new Vector3(0f, 0.9f, 0f));
            station.slot1ProgressBar = pb1;
            station.slot2ProgressBar = pb2;
            pb1.Show(false);
            pb2.Show(false);

            // World Label
            CreateWorldLabel(stoveObj.transform, new Vector3(0f, 1.3f, 0f), "STOVE (2 SLOTS)\n(Cook Meat - 6s)", new Color(1.0f, 0.6f, 0.2f), new Color(0.05f, 0.08f, 0.14f, 0.92f), new Vector2(250, 60));
        }

        private void CreateStoveBurner(Transform parent, Vector3 localPos)
        {
            GameObject burner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            burner.name = "BurnerPlate";
            burner.transform.SetParent(parent, false);
            burner.transform.localPosition = localPos;
            burner.transform.localScale = new Vector3(0.7f, 0.02f, 0.7f);
            SetMaterialColor(burner, new Color(0.12f, 0.12f, 0.15f));
            DestroyImmediate(burner.GetComponent<Collider>());
        }

        private void CreateTrash(Transform parent, Vector3 pos)
        {
            GameObject trashObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trashObj.name = "TrashCan";
            trashObj.transform.SetParent(parent);
            trashObj.transform.position = pos;
            trashObj.transform.localScale = new Vector3(1.3f, 0.9f, 1.3f);
            SetMaterialColor(trashObj, new Color(0.2f, 0.22f, 0.26f));

            TrashStation station = trashObj.AddComponent<TrashStation>();
            station.stationName = "Trash Can (Discard Item)";

            // World Label
            CreateWorldLabel(trashObj.transform, new Vector3(0f, 1.3f, 0f), "TRASH CAN\n(Discard Item)", new Color(1.0f, 0.4f, 0.4f), new Color(0.05f, 0.08f, 0.14f, 0.92f), new Vector2(230, 50));
        }

        private GameObject CreateWorldLabel(Transform parent, Vector3 localPos, string text, Color textColor, Color bgColor, Vector2 sizeDelta)
        {
            GameObject labelCanvasObj = new GameObject("WorldLabelCanvas");
            labelCanvasObj.transform.SetParent(parent, false);
            labelCanvasObj.transform.localPosition = localPos;
            labelCanvasObj.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            Canvas canvas = labelCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            labelCanvasObj.transform.localScale = Vector3.one * 0.009f;

            RectTransform canvasRT = canvas.GetComponent<RectTransform>();
            canvasRT.sizeDelta = sizeDelta;

            Image bg = labelCanvasObj.AddComponent<Image>();
            bg.color = bgColor;

            GameObject txtObj = new GameObject("LabelText");
            txtObj.transform.SetParent(labelCanvasObj.transform, false);

            Text txt = txtObj.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = 20;
            txt.fontStyle = FontStyle.Bold;
            txt.color = textColor;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", 20);

            RectTransform txtRT = txt.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;

            return labelCanvasObj;
        }

        private void SetupPlayer(Transform parent, Vector3 pos)
        {
            GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObj.name = "PlayerChef";
            playerObj.transform.SetParent(parent);
            playerObj.transform.position = pos;
            playerObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            SetMaterialColor(playerObj, new Color(0.95f, 0.95f, 0.95f));

            GameObject hat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hat.name = "ChefHat";
            hat.transform.SetParent(playerObj.transform);
            hat.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            hat.transform.localScale = new Vector3(0.8f, 0.4f, 0.8f);
            SetMaterialColor(hat, Color.white);
            DestroyImmediate(hat.GetComponent<Collider>());

            Rigidbody rb = playerObj.GetComponent<Rigidbody>();
            if (rb == null) rb = playerObj.AddComponent<Rigidbody>();
            rb.freezeRotation = true;

            PlayerController controller = playerObj.AddComponent<PlayerController>();
            controller.moveSpeed = 7.0f;

            PlayerInteraction interaction = playerObj.AddComponent<PlayerInteraction>();

            GameObject holdPointObj = new GameObject("ItemHoldPoint");
            holdPointObj.transform.SetParent(playerObj.transform);
            holdPointObj.transform.localPosition = new Vector3(0f, 0.6f, 0.7f);
            interaction.holdPoint = holdPointObj.transform;
        }

        private ProgressBarUI CreateProgressBar(Transform parent, Vector3 localPos)
        {
            GameObject pbCanvasObj = new GameObject("ProgressBarCanvas");
            pbCanvasObj.transform.SetParent(parent);
            pbCanvasObj.transform.localPosition = localPos;
            pbCanvasObj.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            Canvas canvas = pbCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            pbCanvasObj.transform.localScale = Vector3.one * 0.01f;
            RectTransform canvasRT = canvas.GetComponent<RectTransform>();
            canvasRT.sizeDelta = new Vector2(120, 24);

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(pbCanvasObj.transform, false);
            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(bgObj.transform, false);
            Image fill = fillObj.AddComponent<Image>();
            fill.color = new Color(0.2f, 0.9f, 0.4f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            RectTransform fillRT = fill.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.sizeDelta = Vector2.zero;

            ProgressBarUI pb = pbCanvasObj.AddComponent<ProgressBarUI>();
            pb.fillImage = fill;
            pb.canvas = canvas;
            return pb;
        }

        private void SetupUI(Transform parent, GameManager gameMgr)
        {
            GameObject canvasObj = new GameObject("Canvas_MainUI");
            canvasObj.transform.SetParent(parent);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // EventSystem
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            UIManager uiMgr = canvasObj.AddComponent<UIManager>();

            // ----------------------------------------------------
            // 1. START WINDOW PANEL
            // ----------------------------------------------------
            GameObject startPanel = CreatePanel(canvasObj.transform, "StartWindowPanel", new Color(0.05f, 0.07f, 0.1f, 0.95f));
            
            // Inner Card
            GameObject startCard = CreatePanel(startPanel.transform, "Card", new Color(0.1f, 0.13f, 0.18f, 0.95f));
            RectTransform startCardRT = startCard.GetComponent<RectTransform>();
            startCardRT.anchorMin = new Vector2(0.2f, 0.1f);
            startCardRT.anchorMax = new Vector2(0.8f, 0.9f);
            startCardRT.sizeDelta = Vector2.zero;

            CreateText(startCard.transform, "Title", "YES CHEF!", 52, FontStyle.Bold, new Color(1.0f, 0.85f, 0.2f), new Vector2(0f, 320f), new Vector2(700, 80));
            CreateText(startCard.transform, "SubTitle", "Tentworks Dev Test - Kitchen Management", 24, FontStyle.Bold, Color.white, new Vector2(0f, 250f), new Vector2(700, 40));

            string controlsText = "HOW TO PLAY & CONTROLS:\n\n" +
                "• WASD / Arrow Keys : Move Chef around the kitchen\n" +
                "• E / Space / Left Click : Interact with Stations / Items\n" +
                "• Refrigerator : Acquire raw ingredients (Veg, Cheese, Meat)\n" +
                "• Cutting Table : Chop Vegetables (takes 2 seconds)\n" +
                "• Stove : Cook Meat (2 slots, 6 seconds per slot)\n" +
                "• Customer Windows : Fulfill orders before timer runs down!\n" +
                "• Trash Can : Throw away mistakes\n" +
                "• Escape / P : Pause Game";

            CreateText(startCard.transform, "ControlsInfo", controlsText, 20, FontStyle.Normal, new Color(0.85f, 0.9f, 0.95f), new Vector2(0f, 20f), new Vector2(750, 320));

            Button startBtn = CreateButton(startCard.transform, "StartButton", "START GAME", new Vector2(0f, -300f), new Vector2(300, 70), new Color(0.2f, 0.75f, 0.35f));
            uiMgr.startWindowPanel = startPanel;
            uiMgr.startButton = startBtn;

            // ----------------------------------------------------
            // 2. HUD PANEL
            // ----------------------------------------------------
            GameObject hudPanel = CreatePanel(canvasObj.transform, "HUDPanel", Color.clear);

            // Top Bar Container
            GameObject topBar = CreatePanel(hudPanel.transform, "TopBar", new Color(0.06f, 0.08f, 0.12f, 0.9f));
            RectTransform topBarRT = topBar.GetComponent<RectTransform>();
            topBarRT.anchorMin = new Vector2(0f, 0.91f);
            topBarRT.anchorMax = new Vector2(1f, 1f);
            topBarRT.sizeDelta = Vector2.zero;

            Text scoreTxt = CreateText(topBar.transform, "ScoreText", "SCORE: 0", 26, FontStyle.Bold, Color.white, new Vector2(-450f, 0f), new Vector2(300, 50));
            Text highScoreTxt = CreateText(topBar.transform, "HighScoreText", "HIGH SCORE: 0", 24, FontStyle.Bold, new Color(1.0f, 0.85f, 0.2f), new Vector2(-100f, 0f), new Vector2(320, 50));
            Text timerTxt = CreateText(topBar.transform, "TimerText", "TIME: 03:00", 26, FontStyle.Bold, new Color(0.3f, 0.95f, 1f), new Vector2(250f, 0f), new Vector2(250, 50));

            Button pauseBtn = CreateButton(topBar.transform, "PauseButton", "PAUSE", new Vector2(550f, 0f), new Vector2(140, 46), new Color(0.25f, 0.3f, 0.38f));

            // Interaction Prompt Container at Bottom Center
            GameObject promptContainer = CreatePanel(hudPanel.transform, "PromptContainer", new Color(0.04f, 0.05f, 0.08f, 0.92f));
            RectTransform promptRT = promptContainer.GetComponent<RectTransform>();
            promptRT.anchorMin = new Vector2(0.25f, 0.03f);
            promptRT.anchorMax = new Vector2(0.75f, 0.12f);
            promptRT.sizeDelta = Vector2.zero;

            Text promptTxt = CreateText(promptContainer.transform, "PromptText", "", 24, FontStyle.Bold, new Color(1.0f, 0.9f, 0.3f), Vector2.zero, new Vector2(900, 70));

            uiMgr.hudPanel = hudPanel;
            uiMgr.scoreText = scoreTxt;
            uiMgr.highScoreText = highScoreTxt;
            uiMgr.timerText = timerTxt;
            uiMgr.pauseButton = pauseBtn;
            uiMgr.promptContainerPanel = promptContainer;
            uiMgr.promptText = promptTxt;

            // ----------------------------------------------------
            // 3. PAUSE PANEL
            // ----------------------------------------------------
            GameObject pausePanel = CreatePanel(canvasObj.transform, "PausePanel", new Color(0f, 0f, 0f, 0.85f));
            
            GameObject pauseCard = CreatePanel(pausePanel.transform, "PauseCard", new Color(0.08f, 0.1f, 0.15f, 0.95f));
            RectTransform pauseCardRT = pauseCard.GetComponent<RectTransform>();
            pauseCardRT.anchorMin = new Vector2(0.3f, 0.25f);
            pauseCardRT.anchorMax = new Vector2(0.7f, 0.75f);
            pauseCardRT.sizeDelta = Vector2.zero;

            CreateText(pauseCard.transform, "PauseTitle", "GAME PAUSED", 44, FontStyle.Bold, Color.white, new Vector2(0f, 180f), new Vector2(500, 60));

            Button resumeBtn = CreateButton(pauseCard.transform, "ResumeBtn", "RESUME", new Vector2(0f, 60f), new Vector2(260, 60), new Color(0.2f, 0.75f, 0.35f));
            Button restartBtn = CreateButton(pauseCard.transform, "RestartBtn", "RESTART", new Vector2(0f, -30f), new Vector2(260, 60), new Color(0.85f, 0.5f, 0.1f));
            Button quitBtn = CreateButton(pauseCard.transform, "QuitBtn", "QUIT", new Vector2(0f, -120f), new Vector2(260, 60), new Color(0.85f, 0.2f, 0.2f));

            uiMgr.pausePanel = pausePanel;
            uiMgr.resumeButton = resumeBtn;
            uiMgr.pauseRestartButton = restartBtn;
            uiMgr.pauseQuitButton = quitBtn;

            // ----------------------------------------------------
            // 4. GAME OVER PANEL
            // ----------------------------------------------------
            GameObject gameOverPanel = CreatePanel(canvasObj.transform, "GameOverPanel", new Color(0.08f, 0.04f, 0.04f, 0.95f));
            
            GameObject goCard = CreatePanel(gameOverPanel.transform, "GOCard", new Color(0.12f, 0.08f, 0.08f, 0.95f));
            RectTransform goCardRT = goCard.GetComponent<RectTransform>();
            goCardRT.anchorMin = new Vector2(0.25f, 0.2f);
            goCardRT.anchorMax = new Vector2(0.75f, 0.8f);
            goCardRT.sizeDelta = Vector2.zero;

            CreateText(goCard.transform, "GOTitle", "TIME'S UP!", 52, FontStyle.Bold, new Color(1.0f, 0.3f, 0.3f), new Vector2(0f, 200f), new Vector2(600, 80));

            Text finalScoreTxt = CreateText(goCard.transform, "FinalScore", "Final Score: 0", 32, FontStyle.Bold, Color.white, new Vector2(0f, 100f), new Vector2(600, 50));
            Text goHighScoreTxt = CreateText(goCard.transform, "GOHighScore", "High Score: 0", 26, FontStyle.Bold, new Color(1.0f, 0.85f, 0.2f), new Vector2(0f, 30f), new Vector2(600, 45));

            Text badgeTxt = CreateText(goCard.transform, "NewHighScoreBadge", "★ NEW HIGH SCORE! ★", 30, FontStyle.Bold, new Color(0.3f, 0.95f, 0.4f), new Vector2(0f, -40f), new Vector2(600, 50));

            Button playAgainBtn = CreateButton(goCard.transform, "PlayAgainBtn", "PLAY AGAIN", new Vector2(0f, -160f), new Vector2(280, 64), new Color(0.2f, 0.75f, 0.35f));

            uiMgr.gameOverPanel = gameOverPanel;
            uiMgr.finalScoreText = finalScoreTxt;
            uiMgr.gameOverHighScoreText = goHighScoreTxt;
            uiMgr.newHighScoreBadge = badgeTxt.gameObject;
            uiMgr.playAgainButton = playAgainBtn;

            // Set initial edit-mode panel visibilities
            startPanel.SetActive(true);
            hudPanel.SetActive(false);
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        private GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            Image img = panel.AddComponent<Image>();
            img.color = color;
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            return panel;
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize, FontStyle fontStyle, Color color, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = content;
            txt.fontSize = fontSize;
            txt.fontStyle = fontStyle;
            txt.color = color;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", fontSize);

            RectTransform rt = txt.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            return txt;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            img.color = color;

            Button btn = btnObj.AddComponent<Button>();

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Text btnTxt = CreateText(btnObj.transform, "Text", label, 20, FontStyle.Bold, Color.white, Vector2.zero, sizeDelta);

            return btn;
        }

        private void SetMaterialColor(GameObject go, Color color)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Material mat = new Material(Shader.Find("Standard") ?? Shader.Find("Sprites/Default"));
                mat.color = color;
                mr.sharedMaterial = mat;
            }
        }
    }
}
