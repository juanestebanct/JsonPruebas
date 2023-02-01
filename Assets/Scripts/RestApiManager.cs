using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RestApiManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> YourRawImage= new List<RawImage>();
    [SerializeField] private int user=1;
    [SerializeField] private TextMeshProUGUI usuario;
    [SerializeField] private string myApi = "https://my-json-server.typicode.com/juanestebanct/JsonPruebas";
    [SerializeField] private string apiRickAndMorty = "https://rickandmortyapi.com/api/character/";
    public int[] cards;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GetCharacterVoid(string id)
    {
        if (int.TryParse(id,out _)  )
        {
            StartCoroutine(GetUserInfo(int.Parse(id)));
            Debug.Log(id);
        }
        usuario.text = "Ingresa un numero";
        Debug.Log("ingresa un numero");
    }
    public void GetCharacterClick(int id)
    {
      StartCoroutine(GetUserInfo(id));

 
    }

    IEnumerator GetCharacter(int id,int posicion)
    {
        UnityWebRequest www = UnityWebRequest.Get(apiRickAndMorty + id);
        yield return www.Send();

        if (www.isNetworkError)
        {
            //Debug.Log("NETWORK ERROR:" + www.error);
        }
        else
        {
            //Debug.log(www.GetResponseHeader("content-type"));
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            if (www.responseCode == 200)
            {
                Character Character = JsonUtility.FromJson<Character>(www.downloadHandler.text);

                StartCoroutine(DownloadImage(Character.image, posicion));
                
               // Debug.Log("funciona");
            }
            else
            {
                string mensaje = "Status:" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError:" + www.error;
                Debug.Log(mensaje);
            }

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
    IEnumerator GetUserInfo(int user)
    {
        UnityWebRequest www = UnityWebRequest.Get(myApi+ "/users/"+ user);
        yield return www.Send();

        if (www.isNetworkError)
        {
            //Debug.Log("NETWORK ERROR:" + www.error);
        }
        else
        {
         
           // Debug.Log(www.downloadHandler.text);
            if (www.responseCode == 200)
            {
                string maso="";
                User User = JsonUtility.FromJson<User>(www.downloadHandler.text);
                Debug.Log(User.name);
                usuario.text = "Usuario encontrado " + " nombre :"+User.name+",";
                int posicion = 0;
                foreach (int cart in User.deck)
                {
                    yield return new WaitForSeconds(0.1f);
                    Debug.Log("carta:"+cart +"nombre"+ User.name);
                    maso +=""+cart+",";
                    StartCoroutine(GetCharacter(cart,posicion));
                    posicion++;
                }
                usuario.text +=" Baraja : " + maso;

            }
            else
            {
                usuario.text = "Este Id no existe en la Api ";
                string mensaje = "Status:" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError:" + www.error;
                Debug.Log(mensaje);
            }

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator DownloadImage(string MediaUrl,int position)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else {
            YourRawImage[position].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
             position++;
            if (position == 5) 
            {
                
                position = 0;

            } 
        }
    }

}

[System.Serializable]
public class CharacterList
{
    public CharacterListInfo info;
    public List<Character> results;
}
public class User
{
    public int id;
    public string name;
    public int[] deck;
}
[System.Serializable]
public class CharacterListInfo
{
    public int count;
    public int pages;
    public string prev;
    public string next;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string specie;
    public string image;
}
