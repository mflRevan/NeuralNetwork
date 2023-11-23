using UnityEngine;
using DG.Tweening;

namespace Default
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Building initialBuilding;

        public Building ActiveBuilding { get; private set; }

        private SpriteRenderer spriteRenderer;


        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (initialBuilding != null)
            {
                PlaceBuilding(initialBuilding);
            }
        }

        public void HighlightForSeconds(float seconds = 0.2f)
        {
            this.DOKill(false);

            float countdown = seconds;

            DOTween.To(() => countdown, x => countdown = x, 0f, seconds)
                .SetTarget(this)
                .OnStart(() => Highlight(true))
                .OnComplete(() => Highlight(false));
        }

        private void Highlight(bool highlight)
        {
            spriteRenderer.color = highlight ? new Color(1f, 0f, 0f, 1f) : new Color(1f, 1f, 1f, 1f);
        }

        public void PlaceBuilding(Building buildingToPlace)
        {
            if (GetActiveBuilding() != null)
            {
                Destroy(ActiveBuilding.gameObject);
                ActiveBuilding = null;
            }

            ActiveBuilding = Instantiate(buildingToPlace.gameObject, transform.position, Quaternion.identity).GetComponent<Building>();
        }

        public void Clear()
        {
            if (ActiveBuilding != null)
            {
                Destroy(ActiveBuilding.gameObject);
                ActiveBuilding = null;
            }
        }

        public Building GetActiveBuilding()
        {
            return ActiveBuilding;
        }
    }
}