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

    public ParticleSystem[] particles;
    [HideInInspector] public GameObject[] characterModels;

    // == 스킬
    // 텔레포트
    bool IsTeleportEnd=true;
    Teleport TeleportSkill;
    

    private void Awake()
    {
        TeleportSkill = new Teleport(0, particles[0], characterModels, camera);
        cameraDefaultFOV = camera.fieldOfView;
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(IsControlWeaponEnable) ControlWand();
       
    }

    private void Update()
    {
        if (!IsTeleportEnd)
        { 
            this.GetComponent<Character>().teleportMove();
        }
        else
        {
           if(camera.fieldOfView >= cameraDefaultFOV) camera.fieldOfView -= Time.deltaTime * cameraSpeed;   
        }
        
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

    

    public void TeleportOnSkillEventStart()
    {
        TeleportSkill.OnSkillEventStart();
    }    

    public void TeleportOnSkillEvent()
    {
        TeleportSkill.OnSkillEvent();
        IsTeleportEnd = false;
    }

    public void TeleportOnSkillEnd()
    {
        TeleportSkill.OnSkillEnd();
        IsTeleportEnd = true;
    }
}



// 미완성
abstract class Skill : MonoBehaviour
{
    public float damage;
    private float Damage { get { return damage; } }
    public GameObject[] CharacterModels;

    public Skill(int damage_,GameObject[] characterModel_)
    {
        damage = damage_;
        CharacterModels = characterModel_;
    }

    public abstract void OnSkillEventStart();

    public abstract void OnSkillEvent();

    public abstract void OnSkillEnd();
}

// 1. 파티클 생성
// 2. 캐릭터 사라짐 , FOV 각도 커짐, 중력영향X, 이동
// 3.  캐릭터 다시 보임, FOV 각도 점점 작아짐 , 파티클 생성

class Teleport : Skill //텔레포트
{
    private ParticleSystem Particle;

    Camera camera;
    private float cameraDefaultFOV;

    ChangeShader CharacterShader;
    ChangeShader WandShader;

    public Teleport(int damage_, ParticleSystem particle, GameObject[] characterModels,Camera Camera_) : base(damage_, characterModels)
    {
        Particle = particle;
        camera = Camera_;
        Particle.gameObject.SetActive(false);
        Particle.Stop();
        cameraDefaultFOV = camera.fieldOfView;

        CharacterShader = new ChangeShader(characterModels[0]);
        WandShader = new ChangeShader(characterModels[1]);
    }

    public override void OnSkillEventStart()
    {
        Particle.Play();
        Particle.gameObject.SetActive(true);
    }

    public override void OnSkillEvent()
    {
        camera.fieldOfView = cameraDefaultFOV * 1.2f;
        WandShader.ChangeTransparent(0.0f, 1);
        CharacterShader.ChangeTransparent(0.0f, 1);
    }

    public override void OnSkillEnd()
    {
        Particle.Play();
        CharacterShader.ChangeTransparent(1.0f, 0);
        WandShader.ChangeTransparent(1.0f, 0);
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


    public class ChangeShader
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

    public void ChangeTransparent(float value, int mode)
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
