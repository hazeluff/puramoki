
using UnityEngine;
using UnityEngine.EventSystems;

public class MBTile : MonoBehaviour {
    private static CustomLogger LOGGER = new CustomLogger(typeof(MBTile).ToString());

    [SerializeField]
    private Tile tile;
    public Tile Tile { get { return tile;  } }

    private MeshRenderer _renderer;
    
    private MBStage stage;

    private void Awake() {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void setStage(MBStage stage) {
        this.stage = stage;
    }

    public void setDefaultColor() {
        setColor(Color.white);
    }

    public void setSelectedColor() {
        setColor(Color.red);
    }

    public void setMoveRangeColor() {
        setColor(Color.cyan);
    }

    private void setColor(Color color) {
        _renderer.material.color = color;
    }

    void OnMouseUpAsButton() {
        if (stage.State != MBStage.ControlState.UNIT_MENU) {
            stage.ClickTile(this);
        }
    }
}
