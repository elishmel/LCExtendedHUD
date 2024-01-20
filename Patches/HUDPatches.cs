using HarmonyLib;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LCExtendedHUD.Utility;
using UnityEngine.UIElements.UIR;
using System.Linq;

namespace LCExtendedHUD.Patches {
    [HarmonyPatch]
    static internal class HUDPatches {

        private static GameObject _reticle;
        private static Image _reticleImage;

        private static GameObject _scrapCounter;
        private static TextMeshProUGUI _scrapTextMesh;

        private static GameObject _conductiveDisplay;
        private static TextMeshProUGUI _conductiveDisplayMesh;

        private static Vector3 CREDIT_OFFSET { get { return new Vector3(0f, -15f, 0f); } }
        private static Vector3 CONDUCTIVE_OFFSET { get { return new Vector3(0f, -30f, 0f); } }

        private const string NOT_CONDUCTIVE_TEXT = "not-conductive";
        private const string CONDUCTIVE_TEXT = "conductive";
        private const string CLEAR_TEXT = "clear";
        private const string PROFIT_TEXT = "credits";

        private const string RETICLE_TEXTURE_PATH = "Assets.circle.png";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager),"Awake")]
        private static void buildHUDPatches(HUDManager __instance) {
            __instance.StartCoroutine(awaitPlayerController());
            drawReticle();
            drawProfit();
            drawConductive();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager),"Update")]
        private static void updateHUDPatches(HUDManager __instance) {
            ExtendedHUD.Log.LogInfo("HUD update");
            updateReticle();
            updateProfit();
            updateConductive();
        }

        private static void drawReticle() {
            GameObject cursor = GameObject.Find("Systems/UI/Canvas/PlayerCursor/Cursor");
            HUDPatches._reticle = GameObject.Instantiate(cursor.gameObject, cursor.transform.parent, false);
            HUDPatches._reticle.name = "Reticle";
            HUDPatches._reticleImage = _reticle.GetComponent<Image>();
            Texture2D reticleTexture = LoadTexture(RETICLE_TEXTURE_PATH);
            Sprite reticleSprite = Sprite.Create(
                reticleTexture,
                new Rect(0, 0, reticleTexture.width, reticleTexture.height),
                Vector2.zero);
            HUDPatches._reticleImage.sprite = reticleSprite;
            HUDPatches._reticleImage.enabled = true;
            HUDPatches._reticle.transform.localScale = new Vector3(-0.036f, -0.036f, 0.364f);
        }

        private static void updateReticle() {
            if (_reticle == null ||
                _reticleImage == null ||
                GameNetworkManager.Instance == null ||
                GameNetworkManager.Instance.localPlayerController == null) {
                    return;
            }

            Image cursor = GameObject.Find("Systems/UI/Canvas/PlayerCursor/Cursor").GetComponent<Image>();

            if (
                (GameNetworkManager.Instance.localPlayerController.isPlayerDead 
                || cursor.enabled) && HUDPatches._reticleImage.enabled
                ) {
                HUDPatches._reticleImage.enabled = (false);
            } else if (!GameNetworkManager.Instance.localPlayerController.isPlayerDead && ! HUDPatches._reticleImage.enabled && !cursor.enabled) {
                ExtendedHUD.Log.LogInfo("Reticle enabled");

                HUDPatches._reticleImage.enabled =(true);
            } 
        }

        private static void drawProfit() {

            GameObject weightCounter = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/WeightUI");
            if (weightCounter == null) {
                ExtendedHUD.Log.LogError($"Cannot obtain weight UI");
                return;
            }

            HUDPatches._scrapCounter = UnityEngine.Object.Instantiate<GameObject>(weightCounter.gameObject, weightCounter.transform.parent, false);

            if (HUDPatches._scrapCounter)
                ExtendedHUD.Log.LogInfo("Weight counter found and copied.");
            else {
                ExtendedHUD.Log.LogError("Unable to create counter copy.");
                return;
            }

            HUDPatches._scrapCounter.name = "Credit counter";

            Vector3 weightCounterLocalPosition = HUDPatches._scrapCounter.transform.localPosition;

            HUDPatches._scrapCounter.transform.localPosition = weightCounterLocalPosition + CREDIT_OFFSET;

            HUDPatches._scrapTextMesh = HUDPatches._scrapCounter.GetComponentInChildren<TextMeshProUGUI>();
            if (!HUDPatches._scrapTextMesh) {
                ExtendedHUD.Log.LogError("No TextMesh found.");
                return;
            } else {
                ExtendedHUD.Log.LogInfo("Text mesh found.");
            }
            HUDPatches._scrapTextMesh.name = "Credits";
            HUDPatches._scrapTextMesh.text = $"0 {PROFIT_TEXT}";

        }
        
        private static void updateProfit() {

            if (GameNetworkManager.Instance.localPlayerController == null
                || GameNetworkManager.Instance.localPlayerController.isPlayerDead
                || HUDPatches._scrapTextMesh == null
                || HUDPatches._scrapCounter == null
                || GameNetworkManager.Instance.localPlayerController.ItemSlots == null
                || GameNetworkManager.Instance.localPlayerController.ItemSlots.Length <= 0)
                return;

            _scrapTextMesh.text = $"{GameNetworkManager.Instance.localPlayerController.ItemSlots.Where(item => item != null && item.itemProperties.isScrap).Sum(item => item.scrapValue)} {PROFIT_TEXT}";

        }

        private static void drawConductive() {

            GameObject weightCounter = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/WeightUI");
            if (weightCounter == null) {
                ExtendedHUD.Log.LogError($"Cannot obtain weight UI");
                return;
            }

            HUDPatches._conductiveDisplay = UnityEngine.Object.Instantiate<GameObject>(weightCounter.gameObject, weightCounter.transform.parent, false);

            if (!HUDPatches._conductiveDisplay) {
                ExtendedHUD.Log.LogError("Unable to create conductive copy.");
                return;
            }

            HUDPatches._conductiveDisplay.name = "Conductive display";

            Vector3 weightCounterLocalPosition = HUDPatches._conductiveDisplay.transform.localPosition;

            HUDPatches._conductiveDisplay.transform.localPosition = weightCounterLocalPosition + CONDUCTIVE_OFFSET;

            HUDPatches._conductiveDisplayMesh = HUDPatches._conductiveDisplay.GetComponentInChildren<TextMeshProUGUI>();
            HUDPatches._conductiveDisplayMesh.name = "Conductive";
            if (!HUDPatches._conductiveDisplayMesh) {
                ExtendedHUD.Log.LogError("No TextMesh found.");
                return;
            } else {
                ExtendedHUD.Log.LogInfo("Text mesh found.");
            }

            HUDPatches._conductiveDisplayMesh.text = NOT_CONDUCTIVE_TEXT;
        }

        private static void updateConductive() {
            if (GameNetworkManager.Instance.localPlayerController == null
                || GameNetworkManager.Instance.localPlayerController.isPlayerDead
                || HUDPatches._conductiveDisplayMesh == null
                || HUDPatches._conductiveDisplay == null
                || GameNetworkManager.Instance.localPlayerController.ItemSlots == null
                || GameNetworkManager.Instance.localPlayerController.ItemSlots.Length <= 0)
                return;

            if (TimeOfDay.Instance.currentLevelWeather != LevelWeatherType.Stormy) {
                _conductiveDisplayMesh.text = CLEAR_TEXT;
                return;
            }


            _conductiveDisplayMesh.text = GameNetworkManager.Instance.localPlayerController.ItemSlots.Count(item => item != null && item.itemProperties.isConductiveMetal) > 0 ? CONDUCTIVE_TEXT : NOT_CONDUCTIVE_TEXT;
        }


        private static IEnumerator awaitPlayerController() {
            yield return new WaitUntil(() => GameNetworkManager.Instance.localPlayerController != null);
        }

        private static Texture2D LoadTexture(string resource) {
            byte[] fileData = ResourceLoader.GetResourceFromCurrentAssemblyAsBytes(resource);
            ExtendedHUD.Log.LogInfo($"Loaded {fileData.Length} bytes from {resource}");
            Texture2D texture = new Texture2D(25, 25);
            texture.LoadImage(fileData);
            return texture;
        }
    }
}
