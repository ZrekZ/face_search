﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp56
{
    class BusinessRecognition
    {
        string Dizin;
        string KlasorAdi;
        string XmlVeriDosyasi;
        public BusinessRecognition()
        {
            KlasorAdi = "Определенные лица";
            XmlVeriDosyasi = "TrainedLabels.xml";
            Dizin = Application.StartupPath + "/" + KlasorAdi + "/";
        }

        public BusinessRecognition(string Dizin, string KlasorAdi)
        {
            this.Dizin = Dizin + "/" + KlasorAdi + "/";
            Eigen_Recog = new Classifier_Train(Dizin, KlasorAdi);
        }

        public BusinessRecognition(string Dizin, string KlasorAdi, string XmlVeriDosyasi)
        {
            this.Dizin = Dizin + "/" + KlasorAdi + "/";
            this.XmlVeriDosyasi = XmlVeriDosyasi;
            Eigen_Recog = new Classifier_Train(Dizin, KlasorAdi, XmlVeriDosyasi);
        }

        
        List<Image<Gray, byte>> resultImages = new List<Image<Gray, byte>>();
        Classifier_Train Eigen_Recog;
        XmlDocument docu = new XmlDocument();
        
       
        public bool SaveTrainingData(Image face_data, string FaceName)
        {
            try
            {
                string NAME_PERSON = FaceName;
                Random rand = new Random();
                bool file_create = true;
                string facename = "face_" + NAME_PERSON + "_" + rand.Next().ToString() + ".jpg";
                while (file_create)
                {

                    if (!File.Exists(Dizin + facename))
                    {
                        file_create = false;
                    }
                    else
                    {
                        facename = "face_" + NAME_PERSON + "_" + rand.Next().ToString() + ".jpg";
                    }
                }


                if (Directory.Exists(Dizin))
                {
                    face_data.Save(Dizin + facename, ImageFormat.Jpeg);
                }
                else
                {
                    Directory.CreateDirectory(Dizin);
                    face_data.Save(Dizin + facename, ImageFormat.Jpeg);
                }
                if (File.Exists(Dizin + XmlVeriDosyasi))
                {
                    //File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", NAME_PERSON.Text + "\n\r");
                    bool loading = true;
                    while (loading)
                    {
                        try
                        {
                            docu.Load(Dizin + XmlVeriDosyasi);
                            loading = false;
                        }
                        catch
                        {
                            docu = null;
                            docu = new XmlDocument();
                            Thread.Sleep(10);
                        }
                    }

                    //получение элемента
                    XmlElement root = docu.DocumentElement;

                    XmlElement face_D = docu.CreateElement("FACE");
                    XmlElement name_D = docu.CreateElement("NAME");
                    XmlElement file_D = docu.CreateElement("FILE");

                    //Добавляем значения для каждого узла
                    //name.Value = textBoxName.Text;
                    //age.InnerText = textBoxAge.Text;
                    //gender.InnerText = textBoxGender.Text;
                    name_D.InnerText = NAME_PERSON;
                    file_D.InnerText = facename;

                    //Создаем элемент Person
                    //person.Attributes.Append(name);
                    face_D.AppendChild(name_D);
                    face_D.AppendChild(file_D);

                    //Добавление элемента New person в конец корневого элемента.
                    root.AppendChild(face_D);

                    //сохранение
                    docu.Save(Dizin + XmlVeriDosyasi);
                    //XmlElement child_element = docu.CreateElement("FACE");
                    //docu.AppendChild(child_element);
                    //docu.Save("TrainedLabels.xml");
                }
                else
                {
                    FileStream FS_Face = File.OpenWrite(Dizin + XmlVeriDosyasi);
                    using (XmlWriter writer = XmlWriter.Create(FS_Face))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Faces_For_Training");

                        writer.WriteStartElement("FACE");
                        writer.WriteElementString("NAME", NAME_PERSON);
                        writer.WriteElementString("FILE", facename);
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                    FS_Face.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void DeleteTrains()
        {
            if (Directory.Exists(Dizin))
            {
                Directory.Delete(Dizin, true);
            }
            Directory.CreateDirectory(Dizin);
        }
    }
}
