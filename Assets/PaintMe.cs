using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class PaintMe : MonoBehaviour
{

    public Color ClearColour;
    public Material PaintShader;
    public RenderTexture PaintTarget;
    private RenderTexture TempRenderTarget;
    private Material ThisMaterial;
    public SteamVR_TrackedObject trackedObj;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Init()
    {
        if (ThisMaterial == null)
            ThisMaterial = this.GetComponent<Renderer>().material;

        //	already setup
        if (PaintTarget != null)
            if (ThisMaterial.mainTexture == PaintTarget)
                return;

        //	copy texture
        if (ThisMaterial.mainTexture != null)
        {
            if (PaintTarget == null)
                PaintTarget = new RenderTexture(ThisMaterial.mainTexture.width, ThisMaterial.mainTexture.height, 0);
            Graphics.Blit(ThisMaterial.mainTexture, PaintTarget);
            ThisMaterial.mainTexture = PaintTarget;
        }
        else
        {
            if (PaintTarget == null)
                PaintTarget = new RenderTexture(1024, 1024, 0);

            //	clear if no existing texture
            Texture2D ClearTexture = new Texture2D(1, 1);
            ClearTexture.SetPixel(0, 0, ClearColour);
            Graphics.Blit(ClearTexture, PaintTarget);
            ThisMaterial.mainTexture = PaintTarget;

        }
    }

    //called for every physics step (fixed interval between calls). Update settings Unity > Edit > Project Settings > Time: 1/90 or 0.011111
    void FixedUpdate()
    {

        RaycastHit hitInfo = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // On GetTouch
        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObj.index); //assign device on every fixed update for the input using the index of the tracked object
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (Physics.Raycast(ray, out hitInfo))
            {
                hitInfo.collider.SendMessage("HandleClick", hitInfo, SendMessageOptions.DontRequireReceiver);
            }
        }

    }

    void HandleClick(RaycastHit Hit)
    {
        Vector2 LocalHit2 = Hit.textureCoord;
        PaintAt(LocalHit2);
    }


    void PaintAt(Vector2 Uv)
    {
        Init();

        if (TempRenderTarget == null)
        {
            TempRenderTarget = new RenderTexture(PaintTarget.width, PaintTarget.height, 0);
        }
        PaintShader.SetVector("PaintUv", Uv);
        Graphics.Blit(PaintTarget, TempRenderTarget);
        Graphics.Blit(TempRenderTarget, PaintTarget, PaintShader);
    }
}
