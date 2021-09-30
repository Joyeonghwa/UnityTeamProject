using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SkillControl : MonoBehaviour
{
    // == 무기 관련 

    public bool IsControlWeaponEnable = true;
    public bool IsFixedWand = true;

    [Range(0, 1)] public float posWeight = 1;
    [Range(0, 1)] public float rotWeight = 1;

    [HideInInspector]  [Range(0, 359)] public float xRot = 0.0f;
    [HideInInspector]  [Range(0, 359)] public float yRot = 0.0f;
    [HideInInspector]  [Range(0, 359)] public float zRot = 0.0f;


    private Animator animator;
    public Transform cameraObjTransform;
    public GameObject wand;
    [HideInInspector] public Camera camera;
    private float cameraDefaultFOV;
    public float cameraSpeed = 2.0f;

    // 스킬
    public ParticleSystem[] particles;
    [HideInInspector] public GameObject[] characterModels;


    ChangeShader CharacterShader;
    ChangeShader WandShader;

    

    private void Awake()
    {
        CharacterShader = new ChangeShader(characterModels[0]);
        WandShader = new ChangeShader(characterModels[1]);
        cameraDefaultFOV = camera.fieldOfView;
    }
    void Start()
    {
        animator = this.GetComponent<Animator>();
        particles[0].Stop();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(IsControlWeaponEnable) ControlWand();
       
    }

    private void Update()
    {
        if (teleportOn)
        {
            camera.fieldOfView -= Time.deltaTime * cameraSpeed;
            this.GetComponent<Character>().teleportMove();
            
        }
        else camera.fieldOfView = cameraDefaultFOV;
    }
    void ControlWand()
    {
        //SetEachWeight
        if (!IsFixedWand)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, cameraObjTransform.position);

            Quaternion handRotation = Quaternion.LookRotation(cameraObjTransform.position - transform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
        }

       
        //LookAtObj
        {
           animator.SetLookAtWeight(1.0f);
           animator.SetLookAtPosition(cameraObjTransform.position);
        }

        //지팡이
       // wand.GetComponent<Transform>().LookAt(cameraObjTransform.position - new Vector3(0.0f, 90.0f, 0.0f));
    }


    public void Death()
    {
        Debug.Log("DEATH");
    }

    bool teleportOn = false;

    public void TeleportOnSkillEventStart()
    {
        particles[0].Play();
        Debug.Log("텔레포트 애니메이션 이벤트 시작");

    }    

    public void TeleportOnSkillEvent()
    {
        WandShader.ChangeTransparent(0.0f, 1);
        CharacterShader.ChangeTransparent(0.0f, 1);
        Debug.Log("텔레포트 애니메이션 이벤트중");
        teleportOn = true;
    }

    public void TeleportOnSkillEnd()
    {
        Debug.Log("텔레포트 애니메이션 이벤트 끝");
       CharacterShader.ChangeTransparent(1.0f,0);
       WandShader.ChangeTransparent(1.0f,0);
        teleportOn = false;
    }
}

class ChangeShader
{
    private Material mat;
    private Shader shaderStandard;
    private Renderer renderer;
    private GameObject characterModel;

    public ChangeShader(GameObject characterModel_)
    {
        characterModel = characterModel_;

        renderer = characterModel.GetComponent<Renderer>();
        mat = renderer.material;
        shaderStandard = Shader.Find("Standard");
        if (!shaderStandard) Debug.Log("shaderStandard not Found");

        if (!characterModel.GetComponent<SkinnedMeshRenderer>())
        {
            MeshRenderer meshrenderer = characterModel.GetComponent<MeshRenderer>();
            meshrenderer.material.shader = shaderStandard;
        }
        else
        {
            SkinnedMeshRenderer meshrenderer = characterModel.GetComponent<SkinnedMeshRenderer>();
            meshrenderer.material.shader = shaderStandard;
        }
        
      

     


    }

    public void ChangeTransparent(float value ,int mode)
    {
        mat.SetColor("_Color", new Color(this.mat.color.r, this.mat.color.g, this.mat.color.b, value));

        if (mode == 0) //default
        {
            mat.SetFloat("_Mode", 0); // opaque            
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.renderQueue = -1;
        }
        else if (mode == 1)
        {
            mat.SetFloat("_Mode", 3); // transparent
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
        }

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");

    }

}

// 미완성
abstract class Skill : MonoBehaviour
{
    public float damage;
    private float Damage { get { return damage; } }
    public GameObject CharacterModel;

    public Skill(int damage_,GameObject characterModel)
    {
        damage = damage_;
        CharacterModel = characterModel;
    }

}
/*
class MainAttack : Skill //주공격(레이저빔쏘기)
{
    public MainAttack(int damage_) : base(damage_){ }
}

class SubAttack : Skill //보조공격(산탄총 공격)
{
    public SubAttack(int damage_) : base(damage_) { }
}

class VSkill : Skill // 근접공격(타격)
{
    public VSkill(int damage_) : base(damage_) { }
}

class QSkill : Skill // 포탑생성
{
    public QSkill(int damage_) : base(damage_) { }
}

class ESkill : Skill // 블랙홀
{
    public ESkill(int damage_) : base(damage_) { }
}
*/
class Teleport : Skill //텔레포트
{
    public float TeleportMoveSpeed;
    public ParticleSystem Particle;

    public Teleport(int damage_, ParticleSystem particle,GameObject characterModel) : base(damage_, characterModel)
    {
        Particle = particle;
        Particle.gameObject.SetActive(false);
    }

    public void OnSkillEvent()
    {
        Debug.Log("텔레포트");
    }
}