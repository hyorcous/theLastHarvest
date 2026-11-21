using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public static bool isDialogueActive = false;

    [TextArea(2, 5)]
    public string[] dialogueLines = new string[]
    {
        "<b>Eski Fener Bekçisi:</b>\n\n— İş için gelmedin herhalde...\n<i>(Hüzünlü bir gülümsemeyle sana bakar)</i>",
        "<b>Ölüm:</b>\n\n— Aslında... iş için geldim.",
        "<b>Eski Fener Bekçisi:</b>\n\n— ...",
        "<b>Ölüm:</b>\n\n— ...",
        "<b>Eski Fener Bekçisi:</b>\n\n— Her gün ekim yapıp tarlayı sulaman lazım. Kalan çevre düzenlemelerinde de bana yardım etmeni istiyorum.",
        "<b>Ölüm:</b>\n\n— Tamam..",
        "<b>Eski Fener Bekçisi:</b>\n\n— Burası hayvanlar için bir sığınak, genelde hasta ve yaşlı hayvanlardan oluşuyor. Onlarla ben ilgileniyorum. Şu anlık bu söylediklerimi yapsan yeterli.",
        "<b>Eski Fener Bekçisi:</b>\n\n— İleride başka isteklerim de olabilir. Gün sonunda yardımının karşılığında sana sıcak bir kap yemek veririm. Boş bir odam var, oraya da istediğin gibi yerleş.",
        "<b>Ölüm:</b>\n\n— Teşekkür ederim...",
        "<b>Eski Fener Bekçisi:</b>\n\n— Al, bunlar tohumların. İstediğin zaman ekmeye başla. Su kovası da evin yanında duruyor, benim hayvanlarla ilgilenmem lazım. Bir şey olursa ahırda olacağım.\n\n<color=#FFD700><i>[İpucu: 1-Çapa | 2-Tohum | 3-Sulama | 4-Hasat (Sol Tık) | U-Uyu (Ertesi Gün)]</i></color>"
    };

    private int currentLine = 0;
    public bool triggerOnGameStart = true;
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("Mobile_FreeSample_male_1");
        if (player != null) playerTransform = player.transform;

        if (triggerOnGameStart)
        {
            StartDialogue();
        }
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                NextLine();
            }
        }
        else if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < 3f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        isDialogueActive = true;
        currentLine = 0;
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine >= dialogueLines.Length)
        {
            isDialogueActive = false;
        }
    }

    void OnGUI()
    {
        if (!isDialogueActive) return;

        GUIStyle dialogStyle = new GUIStyle(GUI.skin.box);
        dialogStyle.fontSize = 18;
        dialogStyle.wordWrap = true;
        dialogStyle.richText = true;
        dialogStyle.alignment = TextAnchor.UpperLeft;
        dialogStyle.normal.textColor = Color.white;
        dialogStyle.padding = new RectOffset(25, 25, 20, 20);

        float width = Screen.width * 0.75f;
        float height = 160f;
        float x = (Screen.width - width) / 2f;
        float y = Screen.height - height - 40f;

        GUI.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
        GUI.Box(new Rect(x, y, width, height), $"{dialogueLines[currentLine]}\n\n<size=13><color=#AAAAAA><i>[Sol Tık veya Boşluk ile devam et...]</i></color></size>", dialogStyle);
    }
}