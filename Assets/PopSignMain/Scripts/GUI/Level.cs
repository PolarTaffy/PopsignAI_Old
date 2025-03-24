using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level : MonoBehaviour {
  public int number;
  public Text label;
  public GameObject lockimage;
  public Sprite lockSprite;

	void Start () {
        if(number <= PlayerPrefs.GetInt("MaxLevel"))
        {
            lockimage.gameObject.SetActive( false );
            label.text = "" + number;
        }
        else
        {
          GetComponent<Image>().sprite = lockSprite;
          label.text = "";
        }
        int stars = PlayerPrefs.GetInt( string.Format( "Level.{0:000}.StarsCount", number ), 0 );
        if( stars > 0 )
        {
            for( int i = 1; i <= stars; i++ )
            {
                transform.Find( "Star" + i ).gameObject.SetActive( true );
            }
        }
	}

	void Update () {

	}

  public void StartLevel()
  {
      InitScriptName.InitScript.Instance.OnLevelClicked( number );
      if(number % 2 == 0)
        GamePlay.Instance.isPopSignAI = true;
      else
        GamePlay.Instance.isPopSignAI = false;
      UnityEngine.Debug.Log(GamePlay.Instance.isPopSignAI);
  }
}
