using UnityEngine;

public class debugCtrlPts : MonoBehaviour
{
    public planeMovementPath planeProperties;
    public bool debug = false;
    public SpriteRenderer arrCtrlPt, depCtrlPt;
    
    // Update is called once per frame
    void Update()
    {
        if (!debug || planeProperties == null)
        {
            hide();
            return;
        }
        show(arrCtrlPt, planeProperties.arrivalControlPoint);
        show(depCtrlPt, planeProperties.departureControlPoint);
    }

    private void hide()
    {
        setVisible(arrCtrlPt, false);
        setVisible(depCtrlPt, false);
    }

    private void show(SpriteRenderer point, Vector3 position)
    {
        if (point == null) return;
        point.transform.position = position;
        point.enabled = true;
    }

    private void setVisible(SpriteRenderer point, bool show)
    {
        if (point != null) point.enabled = show;
    }
}
