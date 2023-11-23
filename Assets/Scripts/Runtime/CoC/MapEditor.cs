using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Default
{
    public class MapEditor : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask gridMask;
        [SerializeField] private Map map;

        [Header("Buildings")]
        [SerializeField] private Building cannonPrefab;
        [SerializeField] private Building StomperPrefab;
        [SerializeField] private Building mortarPrefab;
        [SerializeField] private Building resourcePrefab;
        [SerializeField] private Building blankPrefab;

        [Header("Resources")]
        [SerializeField] private int initialCannonCount;
        [SerializeField] private int initialStomperCount;
        [SerializeField] private int initialMortarCount;
        [SerializeField] private int initialResourceCount;
        [SerializeField] private int initialBlankCount;

        [Header("UI")]
        [SerializeField] private TMP_Text placementHintText;
        [SerializeField] private TMP_Text cannonCountText;
        [SerializeField] private TMP_Text StomperCountText;
        [SerializeField] private TMP_Text mortarCountText;
        [SerializeField] private TMP_Text resourceCountText;
        [SerializeField] private TMP_Text blankCountText;

        [Header("Input")]
        [SerializeField] private InputActionReference clearInput;
        [SerializeField] private InputActionReference placeStomperInput;
        [SerializeField] private InputActionReference placeCannonInput;
        [SerializeField] private InputActionReference placeMortarInput;
        [SerializeField] private InputActionReference placeResourceInput;
        [SerializeField] private InputActionReference placeBlankInput;

        public bool PlacementEnabled { get; private set; }

        private int cannonCount;
        private int stomperCount;
        private int mortarCount;
        private int resourceCount;
        private int blankCount;

        private Cell currentSelection;

        private void Awake()
        {
            clearInput.action.performed += OnClearInput;
            placeCannonInput.action.performed += OnCannonInput;
            placeStomperInput.action.performed += OnStomperInput;
            placeMortarInput.action.performed += OnMortarInput;
            placeResourceInput.action.performed += OnResourceInput;
            placeBlankInput.action.performed += OnBlankInput;

            UpdateResources(BuildingType.Cannon, initialCannonCount);
            UpdateResources(BuildingType.Stomper, initialStomperCount);
            UpdateResources(BuildingType.Mortar, initialMortarCount);
            UpdateResources(BuildingType.Resource, initialResourceCount);
            UpdateResources(BuildingType.Blank, initialBlankCount);

            EnablePlacement(true);
        }

        private void OnDestroy()
        {
            clearInput.action.performed -= OnClearInput;
            placeCannonInput.action.performed -= OnCannonInput;
            placeMortarInput.action.performed -= OnMortarInput;
            placeResourceInput.action.performed -= OnResourceInput;
            placeBlankInput.action.performed -= OnBlankInput;
            placeStomperInput.action.performed -= OnStomperInput;
        }

        private void Update()
        {
            if (!PlacementEnabled)
            {
                return;
            }

            var ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000f, gridMask, QueryTriggerInteraction.Collide))
            {
                var cell = hit.transform.GetComponent<Cell>();

                cell.HighlightForSeconds();
                currentSelection = cell;
            }
            else
            {
                currentSelection = null;
            }
        }

        private void EnablePlacement(bool enable)
        {
            PlacementEnabled = enable;

            placementHintText.text = PlacementEnabled ? "Press the first letter of the building to place it!" : "Press Clear to enable placement!";
        }

        public void SaveCurrentLayout()
        {
            map.SaveCurrentMapData();
        }

        public void LoadRandomLayout()
        {
            EnablePlacement(false);

            map.Clear();
            map.InitializeRandomMapFromDatabase();
        }

        public void ClearMap()
        {
            EnablePlacement(true);

            map.Clear();

            cannonCount = initialCannonCount;
            stomperCount = initialStomperCount;
            mortarCount = initialMortarCount;
            resourceCount = initialResourceCount;
            blankCount = initialBlankCount;

            UpdateResources(BuildingType.Cannon, 0);
            UpdateResources(BuildingType.Stomper, 0);
            UpdateResources(BuildingType.Mortar, 0);
            UpdateResources(BuildingType.Resource, 0);
            UpdateResources(BuildingType.Blank, 0);
        }

        private void PlaceBuilding(Building buildingToPlace)
        {
            print("place building " + buildingToPlace.gameObject.name);

            if (currentSelection == null)
            {
                return;
            }

            var activeBuilding = currentSelection.GetActiveBuilding();

            if (activeBuilding != null && activeBuilding.Type == BuildingType.Base)
            {
                return;
            }

            if (activeBuilding != null)
            {
                UpdateResources(activeBuilding.Type, 1);
                currentSelection.Clear();
            }

            UpdateResources(buildingToPlace.Type, -1);
            currentSelection.PlaceBuilding(buildingToPlace);
        }


        private void UpdateResources(BuildingType type, int modification)
        {
            switch (type)
            {
                case BuildingType.Cannon:
                    cannonCount = Mathf.Clamp(cannonCount + modification, 0, 10000);
                    cannonCountText.text = $"Cannons: {cannonCount}";
                    break;

                case BuildingType.Stomper:
                    stomperCount = Mathf.Clamp(stomperCount + modification, 0, 10000);
                    StomperCountText.text = $"Stompers: {stomperCount}";
                    break;

                case BuildingType.Mortar:
                    mortarCount = Mathf.Clamp(mortarCount + modification, 0, 10000);
                    mortarCountText.text = $"Mortars: {mortarCount}";
                    break;

                case BuildingType.Resource:
                    resourceCount = Mathf.Clamp(resourceCount + modification, 0, 10000);
                    resourceCountText.text = $"Resource buildings: {resourceCount}";
                    break;

                case BuildingType.Blank:
                    blankCount = Mathf.Clamp(blankCount + modification, 0, 10000);
                    blankCountText.text = $"Placeholder buildings: {blankCount}";
                    break;
            }
        }

        private void OnClearInput(InputAction.CallbackContext obj)
        {
            if (currentSelection == null)
            {
                return;
            }

            var activeBuilding = currentSelection.GetActiveBuilding();

            if (activeBuilding == null || activeBuilding.Type == BuildingType.Base)
            {
                return;
            }

            UpdateResources(activeBuilding.Type, 1);
            currentSelection.Clear();
        }

        private void OnCannonInput(InputAction.CallbackContext obj)
        {
            if (cannonCount > 0)
            {
                PlaceBuilding(cannonPrefab);
            }
        }

        private void OnMortarInput(InputAction.CallbackContext obj)
        {
            if (mortarCount > 0)
            {
                PlaceBuilding(mortarPrefab);
            }
        }

        private void OnResourceInput(InputAction.CallbackContext obj)
        {
            if (resourceCount > 0)
            {
                PlaceBuilding(resourcePrefab);
            }
        }

        private void OnBlankInput(InputAction.CallbackContext obj)
        {
            if (blankCount > 0)
            {
                PlaceBuilding(blankPrefab);
            }
        }

        private void OnStomperInput(InputAction.CallbackContext obj)
        {
            if (stomperCount > 0)
            {
                PlaceBuilding(StomperPrefab);
            }
        }
    }
}