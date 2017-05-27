using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DeusUtility.TextGen
{
    public enum Alignment { Left, Center, Right }
    public class TextGenerator : MonoBehaviour
    {
        public GameObject S_leter;
        public GameObject O_leter;
        public GameObject M_leter;
        public GameObject E_leter;
        public GameObject T_leter;
        public GameObject H_leter;
        public GameObject I_leter;
        public GameObject N_leter;
        public GameObject G_leter;
        public Material leterMaterial;
        public Alignment verticalAlignment;
        public Alignment horisontalAlignment;
        public GameObject textPane;
        public float Interval;
        public float scale;
        private void Start()
        {
            GenerateText("Something", this.transform.position, this.transform.rotation);
        }
        public void GenerateText(string text, Vector3 position, Quaternion rotation)
        {
            Transform parent = Instantiate(textPane, position, rotation).transform;
            float shift = 0;
            GameObject leterPrefab;
            GameObject leterModel;
            Vector3 leterScale = new Vector3(scale * -10, scale * 10, scale);
            for (int i = text.Length - 1; i >= 0; i--)
            {
                switch (text[i])
                {
                    case 's':
                    case 'S':
                        {
                            leterPrefab = S_leter;
                            shift -= 0.6f;
                            break;
                        }
                    case 'o':
                    case 'O':
                        {
                            leterPrefab = O_leter;
                            shift -= 0.7f;
                            break;
                        }
                    case 'm':
                    case 'M':
                        {
                            leterPrefab = M_leter;
                            shift -= 0.9f;
                            break;
                        }
                    case 'e':
                    case 'E':
                        {
                            leterPrefab = E_leter;
                            shift -= 0.7f;
                            break;
                        }
                    case 't':
                    case 'T':
                        {
                            leterPrefab = T_leter;
                            shift -= 0.7f;
                            break;
                        }
                    case 'h':
                    case 'H':
                        {
                            leterPrefab = H_leter;
                            shift -= 0.7f;
                            break;
                        }
                    case 'i':
                    case 'I':
                        {
                            leterPrefab = I_leter;
                            shift -= 0.3f;
                            break;
                        }
                    case 'n':
                    case 'N':
                        {
                            leterPrefab = N_leter;
                            shift -= 0.7f;
                            break;
                        }
                    case 'g':
                    case 'G':
                        {
                            leterPrefab = G_leter;
                            shift -= 0.7f;
                            break;
                        }
                    default:
                        {
                            leterPrefab = S_leter;
                            shift -= 0.7f;
                            break;
                        }
                }
                leterModel = Instantiate(leterPrefab, parent);
                leterModel.transform.position = parent.position + parent.right * shift;
                leterModel.transform.rotation = rotation;
                leterModel.transform.localScale = leterScale;
                leterModel.GetComponent<MeshRenderer>().material = leterMaterial;
                shift -= Interval;
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                switch (horisontalAlignment)
                {
                    case Alignment.Left:
                        {
                            parent.GetChild(i).transform.position += parent.right * (-shift);
                            break;
                        }
                    case Alignment.Center:
                        {
                            parent.GetChild(i).transform.position += parent.right * (-shift / 2);
                            break;
                        }
                    case Alignment.Right:
                        {
                            break;
                        }
                }
                switch (verticalAlignment)
                {
                    case Alignment.Left:
                        {
                            parent.GetChild(i).transform.position += parent.up * (-0.7f);
                            break;
                        }
                    case Alignment.Center:
                        {
                            parent.GetChild(i).transform.position += parent.up * (-0.35f);
                            break;
                        }
                    case Alignment.Right:
                        {
                            break;
                        }
                }
            }
            if (parent.FindChild("Base"))
            {
                parent.FindChild("Base").transform.localScale = new Vector3(-shift, 1, 1);
            }
        }
    }
}
