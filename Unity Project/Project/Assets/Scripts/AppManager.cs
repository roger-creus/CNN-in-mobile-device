using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class AppManager : MonoBehaviour
{

    public ModelManager mm;
	public Dictionary<int, string> idx_to_labels = new Dictionary<int, string>();


    public GameObject predictionPanel;
    public Text prediction_text_1;
    public Text prediction_text_2;
    public Text prediction_text_3;

    public Image prediction_img_1;
    public Image prediction_img_2;
    public Image prediction_img_3;



    // Start is called before the first frame update
    void Start()
    {
        idx_to_labels = ReadLabels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPredictionTexts(double[] best_idxs, double[] best_probs)
    {
        predictionPanel.SetActive(true);
        prediction_text_1.text = "1) " + idx_to_labels[(int)best_idxs[0]] + ": " + (best_probs[0] * 100).ToString("N2") + "%";
        prediction_text_2.text = "2) " + idx_to_labels[(int)best_idxs[1]] + ": " + (best_probs[1] * 100).ToString("N2") + "%";
        prediction_text_3.text = "3) " + idx_to_labels[(int)best_idxs[2]] + ": " + (best_probs[2] * 100).ToString("N2") + "%";

        prediction_img_1.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[0].ToString(), typeof(Sprite));
        prediction_img_2.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[1].ToString(), typeof(Sprite));
        prediction_img_3.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[2].ToString(), typeof(Sprite));
    }

    public Dictionary<int, string> ReadLabels()
    {
        Dictionary<int, string> IDXS = new Dictionary<int, string>();


        // Read the file IN PC
        /*
        System.IO.StreamReader file = new System.IO.StreamReader("Assets/Scripts/imgnet-labels.txt");
        while ((line = file.ReadLine()) != null)
        {
            string[] line_to_words = line.Split(' ');
            int index = int.Parse(line_to_words[line_to_words.Length - 1]);

            string fullname = "";
            for (int i = 0; i < line_to_words.Length - 1; i++)
            {
                fullname = fullname + line_to_words[i];
            }

            IDXS[index] = fullname;
        }
        */
        
        TextAsset f = (TextAsset)Resources.Load("traffic-sign-labels", typeof(TextAsset));
        

        if (f != null)
        {
            string text = f.text;
            string[] lines = text.Split(System.Environment.NewLine.ToCharArray());
            
            foreach (string line in lines)
            {
                string[] line_to_words = line.Split(' ');
                int index = int.Parse(line_to_words[line_to_words.Length - 1]);
                string fullname = "";
                for (int i = 0; i < line_to_words.Length - 1; i++)
                {
                    fullname += line_to_words[i] + " ";
                }

                IDXS[index] = fullname;

            }
        }

        Debug.Log(IDXS.Keys.Count);

        return IDXS;

    }
}

