using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{

    [SerializeField]
    private AssetReferenceGameObject[] spaceShip;
    [SerializeField]
    private AssetReferenceT<AudioClip> soundBGM;
    [SerializeField]
    private AssetReferenceSprite flagSprite;

    [SerializeField]
    private GameObject BGMobj;
    [SerializeField]
    private Image logoImage;

    private List<GameObject> gameObjects = new List<GameObject>();
    private AudioSource bgmSound;

    void Start()
    {
        StartCoroutine(InitAddressable());
    }

    IEnumerator InitAddressable() 
    {
        var init = Addressables.InitializeAsync();
        yield return init; 
    }

    public void Button_SpawnObject()
    {
        for (int i = 0; i < spaceShip.Length; i++)
        {
            spaceShip[i].InstantiateAsync().Completed += (obj) =>
            {
                gameObjects.Add(obj.Result);
            };
        }

        soundBGM.LoadAssetAsync().Completed += (clip) =>
        {
            bgmSound = BGMobj.GetComponent<AudioSource>();
            bgmSound.clip = clip.Result;
            bgmSound.loop = true;
            bgmSound.Play();
        };

        flagSprite.LoadAssetAsync().Completed += (img) => 
        {
            logoImage = logoImage.GetComponent<Image>();
            logoImage.sprite = img.Result;
        };
    }

    public void Button_Release()
    {
        soundBGM.ReleaseAsset();
        flagSprite.ReleaseAsset();   // ReleaseAsset <-> LoadAssetAsync

        if (bgmSound != null)
            bgmSound.clip = null;

        if (logoImage.sprite != null)
            logoImage.sprite = null;

        if (gameObjects.Count == 0 )
            return;

        var index = gameObjects.Count - 1;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            Addressables.ReleaseInstance(gameObjects[i]);   //      ReleaseInstance  <->  InstantiateAsync  생성한걸 제거하고 메모리에서 해제함
        }
        
        gameObjects.RemoveAt(index); // List에서도 제거
    }

}
