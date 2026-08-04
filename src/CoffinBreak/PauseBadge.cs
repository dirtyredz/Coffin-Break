using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoffinBreak
{
    /// <summary>
    /// The "time is stopped" badge.
    ///
    /// This lives on the mod's own canvas, which per <c>10-visual-integration.md</c> is exactly
    /// the case that inherits nothing and needs the most scrutiny: font, colour and shape are all
    /// set explicitly from the game's own assets rather than left on Unity defaults.
    /// </summary>
    internal sealed class PauseBadge : MonoBehaviour
    {
        private const float PaddingX = 26f;
        private const float PaddingY = 14f;

        private AfkWatcher watcher;

        private Canvas canvas;
        private RectTransform plateRect;
        private TextMeshProUGUI text;
        private CanvasGroup group;

        private bool built;
        private string lastRendered;
        private float lastFontSize;

        internal void Bind(AfkWatcher owner)
        {
            watcher = owner;
        }

        private void Update()
        {
            var wanted = CoffinBreakPlugin.Enabled.Value &&
                         CoffinBreakPlugin.ShowBadge.Value &&
                         AfkWatcher.IsPaused;

            if (!wanted)
            {
                if (group != null)
                {
                    group.alpha = 0f;
                }
                return;
            }

            if (!built)
            {
                Build();
                if (!built)
                {
                    return;
                }
            }

            group.alpha = 1f;
            Position();

            // Font size is checked alongside the caption because with ShowPausedDuration off the
            // caption never changes, and a size edited in Mod Menu would otherwise never apply.
            var caption = Caption();
            if (caption != lastRendered ||
                !Mathf.Approximately(lastFontSize, CoffinBreakPlugin.BadgeFontSize.Value))
            {
                lastRendered = caption;
                lastFontSize = CoffinBreakPlugin.BadgeFontSize.Value;
                text.text = caption;
                Resize();
            }
        }

        private string Caption()
        {
            if (!CoffinBreakPlugin.ShowPausedDuration.Value || watcher == null)
            {
                return "Time paused — away";
            }

            var seconds = Mathf.FloorToInt(watcher.PausedSeconds);
            if (seconds < 60)
            {
                return $"Time paused — away {seconds}s";
            }

            var minutes = seconds / 60;
            if (minutes < 60)
            {
                return $"Time paused — away {minutes}m";
            }

            return $"Time paused — away {minutes / 60}h {minutes % 60}m";
        }

        private void Build()
        {
            var canvasGo = new GameObject("CoffinBreak_BadgeCanvas");
            canvasGo.transform.SetParent(transform, false);
            DontDestroyOnLoad(canvasGo);

            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // Above the game's HUD, and above Plant Peek's hover at 500, because a message about
            // the clock being stopped is useless if something covers it.
            canvas.sortingOrder = 600;
            canvasGo.AddComponent<CanvasScaler>();

            group = canvasGo.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.interactable = false;
            group.alpha = 0f;

            var plate = new GameObject("Plate");
            plate.transform.SetParent(canvasGo.transform, false);

            var image = plate.AddComponent<Image>();
            image.sprite = PanelSprite.Get();
            image.type = Image.Type.Sliced;
            image.raycastTarget = false;

            plateRect = plate.GetComponent<RectTransform>();

            var label = new GameObject("Label");
            label.transform.SetParent(plate.transform, false);

            text = label.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.raycastTarget = false;
            text.color = GamePalette.NameCream;
            GameFonts.Apply(text, preferOutline: true);

            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            built = true;
        }

        private void Resize()
        {
            text.fontSize = CoffinBreakPlugin.BadgeFontSize.Value;
            text.ForceMeshUpdate();

            var size = text.GetPreferredValues();
            plateRect.sizeDelta = new Vector2(size.x + PaddingX * 2f, size.y + PaddingY * 2f);
        }

        /// <summary>
        /// Re-applied every frame rather than once: the anchor is a live config value, so
        /// changing it in Mod Menu or ConfigurationManager takes effect without a restart.
        /// </summary>
        private void Position()
        {
            Vector2 anchor;
            Vector2 offset;

            const float margin = 32f;

            switch (CoffinBreakPlugin.BadgePosition.Value)
            {
                case CoffinBreakPlugin.BadgeCorner.TopLeft:
                    anchor = new Vector2(0f, 1f);
                    offset = new Vector2(margin, -margin);
                    break;
                case CoffinBreakPlugin.BadgeCorner.TopRight:
                    anchor = new Vector2(1f, 1f);
                    offset = new Vector2(-margin, -margin);
                    break;
                case CoffinBreakPlugin.BadgeCorner.BottomLeft:
                    anchor = new Vector2(0f, 0f);
                    offset = new Vector2(margin, margin);
                    break;
                case CoffinBreakPlugin.BadgeCorner.BottomRight:
                    anchor = new Vector2(1f, 0f);
                    offset = new Vector2(-margin, margin);
                    break;
                case CoffinBreakPlugin.BadgeCorner.BottomCentre:
                    anchor = new Vector2(0.5f, 0f);
                    offset = new Vector2(0f, margin);
                    break;
                default:
                    anchor = new Vector2(0.5f, 1f);
                    offset = new Vector2(0f, -margin);
                    break;
            }

            plateRect.anchorMin = anchor;
            plateRect.anchorMax = anchor;
            plateRect.pivot = anchor;
            plateRect.anchoredPosition = offset;
        }
    }
}
