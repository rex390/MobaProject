using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIPanel : MonoBehaviour {

    //Keep a reference of ui list that will be holding the icons
    GameObject characterListUI;
    bool opened = false;
    float width; //holds the width that the list will need to move based on its size
    void Awake()
    {
        //when the object first loads up we add the onclick listener to the menu button to control it opening and closing
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => MenuControl());
        //set the gameobject to the container of the icons of the characters
        characterListUI = this.transform.GetChild(0).gameObject;
        //set the width up to be the size of the container so it moves accurately so the full list is in view
        width = characterListUI.GetComponent<RectTransform>().rect.width;
    }
    //controls the moving of the menu in or off screen
    void MenuControl()
    {
        //save the intial position 
        Vector3 initialMenuUIPos = this.transform.position;
        //based on if the menu is opened or closed then move the menu accordly
        if (opened == false)
        {
            opened = true;                     
            this.transform.position = new Vector3(initialMenuUIPos.x - width, initialMenuUIPos.y, initialMenuUIPos.z);
            UIListControl();
        }
        else
        {
            opened = false;
            this.transform.position = new Vector3(initialMenuUIPos.x + width, initialMenuUIPos.y, initialMenuUIPos.z);
            UIListControl();
        }
    }
    //populates or removes all the character icons from the container based on if the menu is open or not
    void UIListControl()
    {
        //store a reference to the character UI container
        GameObject CharacterUIContainer = GameObject.FindGameObjectWithTag("ChampionContainerUI");
        if (opened == true)
        {
            //store the prefab of the button that will be used for the character icon
            Button prefab = Resources.Load<Button>("Prefabs/Character");
            //set it to null for the start
            Button characterButton = null;
            //load all the sprite icons for the characters
            Sprite[] characterList = Resources.LoadAll<Sprite>("CharacterImages");
            //go through all the sprites
            for (int counter = 0; counter < characterList.Length; counter++)
            {
                //create the character ui element and then make its parent the container
                characterButton = Instantiate(prefab, characterListUI.transform);
                characterButton.transform.SetParent(CharacterUIContainer.transform);
                //save the name of the character sprite
                string characterName = characterList[counter].name;
                characterButton.name = characterName;
                characterButton.GetComponentInChildren<Text>().text = characterName; //just to help more incase image is bad REMOVE LATER
                //change the icon of this button to the sprite image we got
                characterButton.GetComponent<Image>().sprite = characterList[counter];
                //add a onclick listener that will store the name we just stored so when it is clicked it will load that character
                characterButton.GetComponent<Button>().onClick.AddListener(() => LoadCharacter(characterName));
            }

        }
        else if(opened == false)
        {
            Button[] buttons = CharacterUIContainer.GetComponentsInChildren<Button>();
            foreach(Button button in buttons)
            {
                Destroy(button.gameObject);
            }          
        }
    }

    void LoadCharacter(string characterName_)
    {   
        string FilePath = "Prefabs/Characters/" + characterName_;
        GameObject character = Resources.Load(FilePath) as GameObject;
                
        if(character==null)
        {
            Debug.Log("no character was found in the character folder");
        }
        else
        {
            if(GameObject.FindObjectOfType<Player>()==null)
            {
                createCharacter(character, characterName_);
            }
            else
            {
                
                Destroy(GameObject.FindGameObjectWithTag("Player"));
                //LoadCharacter(characterName_);
                createCharacter(character, characterName_);
            }
        }       
    }

    void createCharacter(GameObject character_,string characterName_)
    {
        GameObject player = Instantiate(character_);
        player.name = "Player-" + characterName_;
        player.AddComponent<Player>();
        player.tag = "Player";
    }
}
