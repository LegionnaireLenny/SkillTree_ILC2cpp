using S1API.PhoneApp;
using S1API.Utils;
using SkillTree.Core.Skills;
using SkillTree.Core.Utilities;
using Il2CppTMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SkillTree.Core.App
{
    /// <summary>
    /// Represents a single skill node in the tree map UI.
    /// Holds references to the node's GameObject, visual components, and underlying Skill data.
    /// </summary>
    public class SkillNode
    {
        public Skill Skill;
        public GameObject Root;
        public RectTransform Rect;
        public Button Button;
        public Image Background;
        public TextMeshProUGUI Label;

        /// <summary>Updates the node's displayed label text.</summary>
        public void SetText(string text) => Label.text = text;

        /// <summary>Updates the node's background color.</summary>
        public void SetBackgroundColor(Color color) => Background.color = color;
    }

    /// <summary>
    /// Phone app that displays skill trees as a scrollable, interactive node graph.
    /// Each category (Stats, Operations, Social, Special) has its own tree with
    /// orthogonal connection lines between parent and child nodes.
    /// </summary>
    public class SkillTreeApp : PhoneApp
    {
        protected override string AppName => "SkillTreeApp";
        protected override string AppTitle => "Skills";
        protected override string IconLabel => "Skills";
        protected override string IconFileName => IconManager.IconApp;
        protected override Sprite IconSprite => IconManager.LoadSprite(IconManager.IconApp);

        private static SkillNode _selectedSkill;

        // Layout constants — horizontal tree (root left, children right)
        private const float NodeWidth = 280f;
        private const float NodeHeight = 36f;
        private const float HSpacing = 330f;
        private const float VSpacing = 46f;
        private const float Padding = 20f;
        private const float LineWidth = 2f;

        // Node colors — named by skill state
        private static readonly Color ColorBackground = new(0.1f, 0.1f, 0.1f);
        private static readonly Color ColorTopBar = new(0.12f, 0.12f, 0.12f);
        private static readonly Color ColorButton = new(0.25f, 0.25f, 0.25f);
        private static readonly Color ColorButtonSelected = new(0.35f, 0.35f, 0.35f);
        private static readonly Color ColorDetails = new(0.15f, 0.15f, 0.15f);
        private static readonly Color ColorMaxLevelSkill = new(0.25f, 0.4f, 0.25f);
        private static readonly Color ColorMaxLevelSkillSelected = new(0.35f, 0.5f, 0.35f);
        private static readonly Color ColorUnlockedSkill = new(0.25f, 0.25f, 0.25f);
        private static readonly Color ColorUnlockedSkillSelected = new(0.35f, 0.35f, 0.45f);
        private static readonly Color ColorLockedSkill = new(0.15f, 0.15f, 0.15f);
        private static readonly Color ColorLockedSkillSelected = new(0.35f, 0.25f, 0.25f);

        // Connection line colors
        private static readonly Color ColorLine = new(0.3f, 0.3f, 0.3f, 0.6f);
        private static readonly Color ColorLineUnlocked = new(0.2f, 0.35f, 0.2f, 0.8f);
        private static readonly Color ColorLineHighlight = new(0.5f, 0.75f, 1f, 0.9f);

        // Text colors
        private static readonly Color ColorText = Color.yellow;
        private static readonly Color ColorTextDim = new(0.7f, 0.7f, 0.5f);
        private static readonly Color ColorAutoUnlock = new(0.6f, 0.8f, 1f);
        private static readonly Color ColorPointsText = new(0.9f, 0.9f, 0.6f);

        // Text Sizes
        private static readonly float HeaderSize = 20f;
        private static readonly float BodySize = 18f;

        private static TMP_FontAsset _cachedFont;

        protected override void OnCreatedUI(GameObject container)
        {
            AcquireFont();

            var mainPanel = CreatePanel("MainPanel", container.transform, ColorBackground,
                Vector2.zero, Vector2.one);

            // === TOP BAR ===
            var topBar = CreatePanel("TopBar", mainPanel.transform, ColorTopBar,
                new Vector2(0f, 0.88f), Vector2.one);
            var topBarRect = topBar.GetComponent<RectTransform>();
            topBarRect.offsetMin = new Vector2(5f, topBarRect.offsetMin.y);
            topBarRect.offsetMax = new Vector2(-5f, -3f);

            var titleGO = CreateTMPText("Title", topBar.transform, "Skills", HeaderSize, ColorText,
                TextAlignmentOptions.MidlineLeft);
            var titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0f);
            titleRect.anchorMax = new Vector2(0.12f, 1f);
            titleRect.offsetMin = new Vector2(8f, 0f);
            titleRect.offsetMax = Vector2.zero;

            var catRow = new GameObject("CategoryRow");
            catRow.transform.SetParent(topBar.transform, false);
            var catRowRect = catRow.AddComponent<RectTransform>();
            catRowRect.anchorMin = new Vector2(0.13f, 0.08f);
            catRowRect.anchorMax = new Vector2(0.65f, 0.92f);
            catRowRect.offsetMin = Vector2.zero;
            catRowRect.offsetMax = Vector2.zero;
            var catLayout = catRow.AddComponent<HorizontalLayoutGroup>();
            catLayout.spacing = 4f;
            catLayout.childForceExpandWidth = true;
            catLayout.childForceExpandHeight = true;
            catLayout.childAlignment = TextAnchor.MiddleCenter;

            var (catStatsGO, catStatsBtn, catStatsTMP) = CreateTabButton("StatsTab",
                catRow.transform, $"Stats ({SkillPoints.StatsPoints})");
            var (catOpsGO, catOpsBtn, catOpsTMP) = CreateTabButton("OpsTab",
                catRow.transform, $"Operations ({SkillPoints.OperationsPoints})");
            var (catSocialGO, catSocialBtn, catSocialTMP) = CreateTabButton("SocialTab",
                catRow.transform, $"Social ({SkillPoints.SocialPoints})");
            var (catSpecialGO, catSpecialBtn, catSpecialTMP) = CreateTabButton("SpecialTab",
                catRow.transform, $"Spec ({SkillPoints.SpecialPoints})");

            catStatsGO.GetComponent<Image>().color = ColorButtonSelected;

            // === MAP PANEL (skill tree area) ===
            var mapPanel = CreatePanel("MapPanel", mainPanel.transform, ColorBackground,
                new Vector2(0f, 0f), new Vector2(0.65f, 0.87f));
            var mapPanelRect = mapPanel.GetComponent<RectTransform>();
            mapPanelRect.offsetMin = new Vector2(5f, 5f);
            mapPanelRect.offsetMax = new Vector2(0f, -2f);
            var scrollRect = mapPanel.AddComponent<PinchableScrollRect>();
            scrollRect.inertia = true;
            scrollRect.initScale = new Vector3(0.8f, 0.8f, 0.8f);
            scrollRect.lowerScale = new Vector3(0.8f, 0.8f, 0.8f);
            scrollRect.upperScale = new Vector3(1f, 1f, 1f);
            scrollRect.decelerationRate = 0.001f;
            scrollRect.elasticity = 0.1f;
            scrollRect.zoomMaxSpeed = 0.05f;

            var viewport = new GameObject("MapViewPort");
            viewport.transform.SetParent(mapPanel.transform, false);
            var viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = new Vector2(0f, 0f);
            viewportRect.anchorMax = new Vector2(1f, 1f);
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = new Vector2(0, -50f);
            var viewImage = viewport.AddComponent<Image>().color = ColorBackground;
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            var treeContent = new GameObject("TreeContent");
            treeContent.transform.SetParent(viewport.transform, false);
            var treeRect = treeContent.AddComponent<RectTransform>();
            treeRect.anchorMin = new Vector2(0f, 0.3f);
            treeRect.anchorMax = new Vector2(1.25f, 1.1f);
            treeRect.offsetMin = Vector2.zero;
            treeRect.offsetMax = Vector2.zero;
            //treeRect.pivot = new Vector2(0.5f, 0.5f);

            scrollRect.viewport = viewportRect;
            scrollRect.content = treeRect;

            var statsContainer = CreateCategoryContainer("StatsContainer", treeContent.transform);
            var opsContainer = CreateCategoryContainer("OpsContainer", treeContent.transform);
            var socialContainer = CreateCategoryContainer("SocialContainer", treeContent.transform);
            var specialContainer = CreateCategoryContainer("SpecialContainer", treeContent.transform);

            var pointsOverlay = CreateTMPText("SkillPointsOverlay", mapPanel.transform, "",
                BodySize, ColorPointsText, TextAlignmentOptions.TopLeft);
            var pointsOverlayRect = pointsOverlay.GetComponent<RectTransform>();
            pointsOverlayRect.anchorMin = new Vector2(0f, 1f);
            pointsOverlayRect.anchorMax = new Vector2(0.5f, 1f);
            pointsOverlayRect.pivot = new Vector2(0f, 1f);
            pointsOverlayRect.offsetMin = new Vector2(8f, -30f);
            pointsOverlayRect.offsetMax = new Vector2(0f, -5f);
            var pointsOverlayTMP = pointsOverlay.GetComponent<TextMeshProUGUI>();
            pointsOverlayTMP.raycastTarget = false;
            pointsOverlayTMP.fontStyle = FontStyles.Bold;

            // === DETAIL PANEL (selected skill info + level button) ===
            var detailPanel = CreatePanel("DetailPanel", mainPanel.transform, ColorDetails,
                new Vector2(0.66f, 0f), new Vector2(1f, 0.87f));
            var detailRect = detailPanel.GetComponent<RectTransform>();
            detailRect.offsetMin = new Vector2(2f, 5f);
            detailRect.offsetMax = new Vector2(-5f, -2f);

            var detailLayout = detailPanel.AddComponent<VerticalLayoutGroup>();
            detailLayout.padding = new RectOffset(8, 8, 10, 10);
            detailLayout.spacing = 6f;
            detailLayout.childForceExpandWidth = true;
            detailLayout.childForceExpandHeight = false;
            detailLayout.childAlignment = TextAnchor.UpperCenter;

            var detailNameGO = CreateTMPText("SkillName", detailPanel.transform, "", HeaderSize,
                ColorText, TextAlignmentOptions.Center);
            var detailNameLayout = detailNameGO.AddComponent<LayoutElement>();
            detailNameLayout.preferredHeight = 26f;
            var detailNameTMP = detailNameGO.GetComponent<TextMeshProUGUI>();

            var detailDescGO = CreateTMPText("SkillDesc", detailPanel.transform, "", BodySize,
                ColorTextDim, TextAlignmentOptions.TopLeft);
            var detailDescLayout = detailDescGO.AddComponent<LayoutElement>();
            detailDescLayout.flexibleHeight = 1f;
            var detailDescTMP = detailDescGO.GetComponent<TextMeshProUGUI>();
            detailDescTMP.enableWordWrapping = true;
            detailDescTMP.overflowMode = TextOverflowModes.Truncate;

            var detailLevelGO = CreateTMPText("SkillLevel", detailPanel.transform, "", BodySize,
                ColorText, TextAlignmentOptions.Center);
            var detailLevelLayout = detailLevelGO.AddComponent<LayoutElement>();
            detailLevelLayout.preferredHeight = 24f;
            var detailLevelTMP = detailLevelGO.GetComponent<TextMeshProUGUI>();

            var autoUnlockGO = CreateTMPText("AutoUnlockInfo", detailPanel.transform, "", BodySize,
                ColorAutoUnlock, TextAlignmentOptions.Center);
            var autoUnlockLayout = autoUnlockGO.AddComponent<LayoutElement>();
            autoUnlockLayout.preferredHeight = 0f;
            var autoUnlockTMP = autoUnlockGO.GetComponent<TextMeshProUGUI>();
            autoUnlockTMP.enableWordWrapping = true;
            autoUnlockTMP.overflowMode = TextOverflowModes.Ellipsis;

            var (levelBtnGO, levelBtn, levelBtnTMP) = CreateTextButton("LevelUpBtn",
                detailPanel.transform, "Level Skill", ColorButton, BodySize);
            var levelBtnLayout = levelBtnGO.AddComponent<LayoutElement>();
            levelBtnLayout.preferredHeight = 40f;

            // === BUILD SKILL TREES ===
            var connectionLines = new Dictionary<(Skill, Skill), List<Image>>();

            List<SkillNode> nodesStats = BuildCategoryTree(SkillTreeData.StatsTree, statsContainer, connectionLines);
            List<SkillNode> nodesOps = BuildCategoryTree(SkillTreeData.OperationsTree, opsContainer, connectionLines);
            List<SkillNode> nodesSocial = BuildCategoryTree(SkillTreeData.SocialTree, socialContainer, connectionLines);
            List<SkillNode> nodesSpecial = BuildCategoryTree(SkillTreeData.SpecialTree, specialContainer, connectionLines);

            List<SkillNode> allNodes = [.. nodesStats, .. nodesOps, .. nodesSocial, .. nodesSpecial];

            SkillCategory activeCategory = SkillCategory.Stats;

            // === INITIAL STATE ===
            _selectedSkill = nodesStats.Count > 0 ? nodesStats[0] : null;
            UpdateDetails();
            SetActiveCategory(statsContainer, opsContainer, socialContainer, specialContainer, statsContainer);
            UpdateAllNodes(allNodes);
            UpdateAllLines();
            UpdatePointsOverlay();

            // === WIRE UP NODE CLICKS ===
            foreach (var node in allNodes)
            {
                var capturedNode = node;
                ButtonUtils.AddListener(node.Button, () =>
                {
                    var prev = _selectedSkill;
                    _selectedSkill = capturedNode;
                    prev?.SetBackgroundColor(GetNodeColor(prev.Skill, false));
                    _selectedSkill.SetBackgroundColor(GetNodeColor(_selectedSkill.Skill, true));
                    UpdateDetails();
                    UpdateAllLines();
                });
            }

            // === WIRE UP LEVEL-UP BUTTON ===
            ButtonUtils.AddListener(levelBtn, () =>
            {
                if (_selectedSkill == null) return;

                bool updateNeeded = false;
                if ((bool)Core.AutoUnlockPrerequisites.BoxedValue)
                {
                    updateNeeded = _selectedSkill.Skill.LevelAndUnlockParents();
                }
                else
                {
                    updateNeeded = _selectedSkill.Skill.IncreaseLevel();
                }

                if (updateNeeded)
                {
                    UpdateDetails();
                    UpdateCategoryText();
                    UpdateAllNodes(allNodes);
                    UpdateAllLines();
                    UpdatePointsOverlay();
                }
            });

            // === WIRE UP CATEGORY TABS ===
            void SwitchCategory(List<SkillNode> nodes, GameObject activeContainer,
                GameObject activeTab, SkillCategory category)
            {
                activeCategory = category;
                _selectedSkill = nodes.Count > 0 ? nodes[0] : null;
                SetActiveCategory(statsContainer, opsContainer, socialContainer,
                    specialContainer, activeContainer);
                SetTabColors(catStatsGO, catOpsGO, catSocialGO, catSpecialGO, activeTab);
                UpdateDetails();
                UpdateCategoryText();
                UpdateAllNodes(allNodes);
                UpdateAllLines();
                UpdatePointsOverlay();
            }

            ButtonUtils.AddListener(catStatsBtn, () =>
                SwitchCategory(nodesStats, statsContainer, catStatsGO, SkillCategory.Stats));
            ButtonUtils.AddListener(catOpsBtn, () =>
                SwitchCategory(nodesOps, opsContainer, catOpsGO, SkillCategory.Operations));
            ButtonUtils.AddListener(catSocialBtn, () =>
                SwitchCategory(nodesSocial, socialContainer, catSocialGO, SkillCategory.Social));
            ButtonUtils.AddListener(catSpecialBtn, () =>
                SwitchCategory(nodesSpecial, specialContainer, catSpecialGO, SkillCategory.Special));

            // === LOCAL FUNCTIONS ===

            void UpdateDetails()
            {
                detailNameTMP.text = _selectedSkill?.Skill.Name ?? "";
                detailDescTMP.text = _selectedSkill?.Skill.Description ?? "";
                detailLevelTMP.text = _selectedSkill != null
                    ? $"Level {_selectedSkill.Skill.CurrentLevel} / {_selectedSkill.Skill.MaxLevel}"
                    : "";

                string autoText = GetAutoUnlockText(_selectedSkill?.Skill);
                autoUnlockTMP.text = autoText;
                autoUnlockLayout.preferredHeight = string.IsNullOrEmpty(autoText) ? 0f : 50f;
                autoUnlockGO.SetActive(!string.IsNullOrEmpty(autoText));
            }

            void UpdateCategoryText()
            {
                catStatsTMP.text = $"Stats ({SkillPoints.StatsPoints})";
                catOpsTMP.text = $"Operations ({SkillPoints.OperationsPoints})";
                catSocialTMP.text = $"Social ({SkillPoints.SocialPoints})";
                catSpecialTMP.text = $"Spec ({SkillPoints.SpecialPoints})";
            }

            void UpdatePointsOverlay()
            {
                int pts = SkillPoints.GetPointsAvailable(activeCategory);
                pointsOverlayTMP.text = $"{pts} Skill Points Available";
            }

            /// <summary>
            /// Recolors all connection lines based on the currently selected skill:
            /// - Highlight (blue): edges on the auto-unlock path from selected skill to root
            /// - Unlocked (dark green): both parent and child are max level
            /// - Default (dim gray): all other edges
            /// </summary>
            void UpdateAllLines()
            {
                // Build the set of edges that would be auto-unlocked if the selected skill is leveled
                var highlightEdges = new HashSet<(Skill, Skill)>();
                if (_selectedSkill != null && !_selectedSkill.Skill.IsParentMaxLevel() &&
                    (bool)Core.AutoUnlockPrerequisites.BoxedValue)
                {
                    var current = _selectedSkill.Skill;
                    while (current.Parent != null)
                    {
                        highlightEdges.Add((current.Parent, current));
                        if (current.Parent.IsMaxLevel()) break;
                        current = current.Parent;
                    }
                }

                foreach (var kvp in connectionLines)
                {
                    var (parentSkill, childSkill) = kvp.Key;
                    var images = kvp.Value;

                    Color lineColor;
                    if (highlightEdges.Contains((parentSkill, childSkill)))
                        lineColor = ColorLineHighlight;
                    else if (parentSkill.IsMaxLevel() && childSkill.IsMaxLevel())
                        lineColor = ColorLineUnlocked;
                    else
                        lineColor = ColorLine;

                    foreach (var img in images)
                        img.color = lineColor;
                }
            }
        }

        // ========== AUTO-UNLOCK INFO ==========

        /// <summary>
        /// Returns a display string listing the skills that would be auto-leveled
        /// if the given skill is leveled with auto-unlock enabled. Returns null if
        /// auto-unlock is disabled, the skill's parent is already unlocked, or the
        /// skill is already maxed.
        /// </summary>
        private static string GetAutoUnlockText(Skill skill)
        {
            if (skill == null) return null;
            if (!(bool)Core.AutoUnlockPrerequisites.BoxedValue) return null;
            if (skill.IsParentMaxLevel()) return null;
            if (skill.IsMaxLevel()) return null;

            var chain = new List<string>();
            var current = skill.Parent;
            while (current != null && !current.IsMaxLevel())
            {
                chain.Add(current.Name);
                current = current.Parent;
            }
            chain.Reverse();

            if (chain.Count == 0) return null;
            return $"Auto-levels: {string.Join(", ", chain)}";
        }

        // ========== TREE LAYOUT & BUILDING ==========

        /// <summary>
        /// Builds the visual tree for a single skill category: computes node positions,
        /// creates node GameObjects, and draws orthogonal connection lines between them.
        /// Connection line Images are stored in <paramref name="connectionLines"/> for
        /// dynamic recoloring on selection changes.
        /// </summary>
        private List<SkillNode> BuildCategoryTree(HashSet<Skill> skillTree,
            GameObject categoryContainer, Dictionary<(Skill, Skill), List<Image>> connectionLines)
        {
            Skill root = skillTree.FirstOrDefault(s => s.Parent == null);
            if (root == null) return new List<SkillNode>();

            var positions = LayoutTreeHorizontal(root);

            // Lines container renders behind nodes (created first = lower sibling index)
            var linesContainer = new GameObject("Lines");
            linesContainer.transform.SetParent(categoryContainer.transform, false);
            var linesRect = linesContainer.AddComponent<RectTransform>();
            linesRect.anchorMin = Vector2.zero;
            linesRect.anchorMax = Vector2.one;
            linesRect.offsetMin = Vector2.zero;
            linesRect.offsetMax = Vector2.zero;

            // Nodes container renders on top of lines (created second = higher sibling index)
            var nodesContainer = new GameObject("Nodes");
            nodesContainer.transform.SetParent(categoryContainer.transform, false);
            var nodesRect = nodesContainer.AddComponent<RectTransform>();
            nodesRect.anchorMin = Vector2.zero;
            nodesRect.anchorMax = Vector2.one;
            nodesRect.offsetMin = Vector2.zero;
            nodesRect.offsetMax = Vector2.zero;

            var nodes = new List<SkillNode>();
            foreach (var kvp in positions)
            {
                var node = CreateSkillNode(kvp.Key, nodesContainer.transform, kvp.Value);
                nodes.Add(node);
            }

            // Each parent-child pair gets its own tracked line segments for individual recoloring
            foreach (var kvp in positions)
            {
                Skill skill = kvp.Key;
                if (skill.Children == null || skill.Children.Count == 0) continue;

                Vector2 parentPos = kvp.Value;
                foreach (var child in skill.Children)
                {
                    if (!positions.TryGetValue(child, out Vector2 childPos)) continue;

                    var lines = CreateConnectionLines(linesContainer.transform, parentPos, childPos);
                    connectionLines[(skill, child)] = lines;
                }
            }

            return nodes;
        }

        /// <summary>
        /// Computes positions for a horizontal tree layout using depth-first traversal.
        /// Root is placed on the left; children branch to the right.
        /// Leaf nodes are assigned sequential Y slots; parent nodes are vertically
        /// centered on their children's Y range.
        /// </summary>
        private static Dictionary<Skill, Vector2> LayoutTreeHorizontal(Skill root)
        {
            var positions = new Dictionary<Skill, Vector2>();
            float nextY = 0f;

            float LayoutRecursive(Skill skill, int depth)
            {
                if (skill.Children == null || skill.Children.Count == 0)
                {
                    float y = nextY;
                    nextY += VSpacing;
                    float x = Padding + depth * HSpacing + NodeWidth / 2f;
                    positions[skill] = new Vector2(x, -(y + Padding));
                    return y;
                }

                float firstChildY = float.MaxValue;
                float lastChildY = float.MinValue;

                foreach (var child in skill.Children)
                {
                    float childY = LayoutRecursive(child, depth + 1);
                    if (childY < firstChildY) firstChildY = childY;
                    if (childY > lastChildY) lastChildY = childY;
                }

                float myY = (firstChildY + lastChildY) / 2f;
                float x2 = Padding + depth * HSpacing + NodeWidth / 2f;
                positions[skill] = new Vector2(x2, -(myY + Padding));
                return myY;
            }

            LayoutRecursive(root, 0);
            return positions;
        }

        // ========== CONNECTION LINES ==========

        /// <summary>
        /// Creates an orthogonal 3-segment connection from a parent node to a single child node:
        /// <list type="number">
        ///   <item>Horizontal stub from parent's right edge to the midpoint X (bus)</item>
        ///   <item>Vertical line from parent's Y to child's Y at the bus X</item>
        ///   <item>Horizontal stub from bus X to child's left edge</item>
        /// </list>
        /// Returns the Image components for later recoloring.
        /// </summary>
        private static List<Image> CreateConnectionLines(Transform parent,
            Vector2 parentPos, Vector2 childPos)
        {
            var images = new List<Image>();

            float parentRightX = parentPos.x + NodeWidth / 2f;
            float childLeftX = childPos.x - NodeWidth / 2f;
            float busX = (parentRightX + childLeftX) / 2f;

            images.Add(CreateLineImage(parent, parentRightX, parentPos.y, busX, parentPos.y));

            if (Mathf.Abs(parentPos.y - childPos.y) > 1f)
                images.Add(CreateLineImage(parent, busX, parentPos.y, busX, childPos.y));

            images.Add(CreateLineImage(parent, busX, childPos.y, childLeftX, childPos.y));

            return images;
        }

        /// <summary>
        /// Creates a single horizontal or vertical line Image between two points.
        /// Orientation is inferred from which dimension (width vs height) is larger.
        /// </summary>
        private static Image CreateLineImage(Transform parent, float x1, float y1, float x2, float y2)
        {
            var go = new GameObject("Line");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2((x1 + x2) / 2f, (y1 + y2) / 2f);

            float width = Mathf.Abs(x2 - x1);
            float height = Mathf.Abs(y2 - y1);
            rect.sizeDelta = width > height
                ? new Vector2(width, LineWidth)
                : new Vector2(LineWidth, height);

            var img = go.AddComponent<Image>();
            img.color = ColorLine;
            img.raycastTarget = false;
            return img;
        }

        // ========== UI CREATION HELPERS ==========

        /// <summary>Creates a panel GameObject with an Image background and anchored RectTransform.</summary>
        private static GameObject CreatePanel(string name, Transform parent, Color color,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        /// <summary>Creates a TextMeshProUGUI element with the cached game font.</summary>
        private static GameObject CreateTMPText(string name, Transform parent, string text,
            float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            if (_cachedFont != null)
                tmp.font = _cachedFont;
            return go;
        }

        /// <summary>Creates a category tab button with a TMPro label.</summary>
        private static (GameObject go, Button btn, TextMeshProUGUI tmp) CreateTabButton(
            string name, Transform parent, string text)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = ColorButton;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelGO = CreateTMPText("Label", go.transform, text, BodySize, ColorText,
                TextAlignmentOptions.Center);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(2f, 0f);
            labelRect.offsetMax = new Vector2(-2f, 0f);

            return (go, btn, labelGO.GetComponent<TextMeshProUGUI>());
        }

        /// <summary>Creates a generic button with an Image background and TMPro label.</summary>
        private static (GameObject go, Button btn, TextMeshProUGUI tmp) CreateTextButton(
            string name, Transform parent, string text, Color bgColor, float fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelGO = CreateTMPText("Label", go.transform, text, fontSize, ColorText,
                TextAlignmentOptions.Center);

            return (go, btn, labelGO.GetComponent<TextMeshProUGUI>());
        }

        /// <summary>Creates a full-anchor container for a skill category's nodes and lines.</summary>
        private static GameObject CreateCategoryContainer(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }

        /// <summary>Creates a clickable skill node positioned at the given coordinates.</summary>
        private static SkillNode CreateSkillNode(Skill skill, Transform parent, Vector2 position)
        {
            var go = new GameObject(skill.Name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(NodeWidth, NodeHeight);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;

            var bg = go.AddComponent<Image>();
            bool isSelected = _selectedSkill != null && _selectedSkill.Skill == skill;
            bg.color = GetNodeColor(skill, isSelected);

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;

            var labelGO = CreateTMPText("Label", go.transform, FormatNodeLabel(skill), BodySize,
                ColorText, TextAlignmentOptions.Center);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(4f, 2f);
            labelRect.offsetMax = new Vector2(-4f, -2f);
            var labelTMP = labelGO.GetComponent<TextMeshProUGUI>();
            labelTMP.enableWordWrapping = false;
            labelTMP.overflowMode = TextOverflowModes.Ellipsis;

            return new SkillNode
            {
                Skill = skill,
                Root = go,
                Rect = rect,
                Button = btn,
                Background = bg,
                Label = labelTMP,
            };
        }

        // ========== STATE HELPERS ==========

        /// <summary>Finds and caches the first available TMP_FontAsset from the scene.</summary>
        private static void AcquireFont()
        {
            if (_cachedFont != null) return;
            var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (fonts != null && fonts.Length > 0)
                _cachedFont = fonts[0];
        }

        /// <summary>Shows only the active category container, hides the rest.</summary>
        private static void SetActiveCategory(GameObject stats, GameObject ops,
            GameObject social, GameObject special, GameObject active)
        {
            stats.SetActive(stats == active);
            ops.SetActive(ops == active);
            social.SetActive(social == active);
            special.SetActive(special == active);
        }

        /// <summary>Highlights the active tab and resets the others to default color.</summary>
        private static void SetTabColors(GameObject statsTab, GameObject opsTab,
            GameObject socialTab, GameObject specialTab, GameObject activeTab)
        {
            statsTab.GetComponent<Image>().color = statsTab == activeTab
                ? ColorButtonSelected : ColorButton;
            opsTab.GetComponent<Image>().color = opsTab == activeTab
                ? ColorButtonSelected : ColorButton;
            socialTab.GetComponent<Image>().color = socialTab == activeTab
                ? ColorButtonSelected : ColorButton;
            specialTab.GetComponent<Image>().color = specialTab == activeTab
                ? ColorButtonSelected : ColorButton;
        }

        /// <summary>Refreshes the label text and background color of every skill node.</summary>
        private static void UpdateAllNodes(List<SkillNode> allNodes)
        {
            foreach (var node in allNodes)
            {
                node.SetText(FormatNodeLabel(node.Skill));
                bool isSelected = _selectedSkill != null && _selectedSkill.Skill == node.Skill;
                node.SetBackgroundColor(GetNodeColor(node.Skill, isSelected));
            }
        }

        private static string FormatNodeLabel(Skill skill) =>
            $"{skill.Name} {skill.CurrentLevel}/{skill.MaxLevel}";

        /// <summary>
        /// Returns the background color for a skill node based on its lock state
        /// (maxed, unlocked, locked) and whether it is currently selected.
        /// </summary>
        private static Color GetNodeColor(Skill skill, bool isSelected)
        {
            if (isSelected)
            {
                if (skill.IsMaxLevel()) return ColorMaxLevelSkillSelected;
                if (skill.IsParentMaxLevel()) return ColorUnlockedSkillSelected;
                return ColorLockedSkillSelected;
            }
            else
            {
                if (skill.IsMaxLevel()) return ColorMaxLevelSkill;
                if (skill.IsParentMaxLevel()) return ColorUnlockedSkill;
                return ColorLockedSkill;
            }
        }
    }
}
