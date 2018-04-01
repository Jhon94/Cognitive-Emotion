using EmotionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace EmotionPlatzi.Web.Util
{
    // Primero se le dieron las propiedades de cognitive a los atributos de EmoFace para posteriormente 
    // Agregar los valores de EmoEmotion en el Collection de EmoEmotion en EmoFace se llama Faces
    public class EmotionHelper
    {
        // Aqui se llaman los servicios de emotion
        public EmotionServiceClient emoClient;

        // Se crea un constructor para instanciar los servicios en la clase
        public EmotionHelper(string key)
        {
            emoClient = new EmotionServiceClient(key);
        }

        // Al tener porpiedades dentro del metodo asyncronas se tiene que agregar un async
        // y es recomendable al nombre del metodo ponerle Async al final para mostrar que es 
        // un metodo async. Se agrega un Task para especificar que se esta usando el modelo 
        // Picture y se crea un parametro Stream imageStream para recivir la imagenes
        public async Task <EmoPicture> DetectAndExtracFacesAsync(Stream imageStream)
        {
            // Se llama Emotion y sus propiedades se reciven como array ya que se pueden almacenar varias imagenes
            // y se almacenarian en un array.
            // Se llaman los servicios de Emotion llamando el emoClient para hacer uso de la propiedad
            // RecognizeAsync y iniciar un reconocimiento en este caso de la imagen(imageStream)
            Emotion[] emotions = await emoClient.RecognizeAsync(imageStream);

            // Se instancia emoPicture para pasarle co
            var emoPicture = new EmoPicture();
            // Se agregan las emociones a emoPicture
            emoPicture.Faces = ExtracFaces(emotions, emoPicture);
            // Se retorna la lista con los valores de emoPictures.EmoFaces que es la collection de EmoFace
            return emoPicture;

        }
        // Se crea el metodo ExtacFaces Para pasarlo como valor en el DetectAndExtracFacesAsync y posteriormente
        // Retornar los valores ingresador como un Stream
        private ObservableCollection<EmoFace> ExtracFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            // Se instancia como una listaObjerbable EmoFace
            // ObservableCollection se usa para traer los datos en una lista y para detectar cambio que
            // Se hagan en el.
            var listaFaces = new ObservableCollection<EmoFace>();
            // Se recorre emotions para irle agregando los valores de cognitive a los atributos de
            // EmoFace
            foreach (var emotion in emotions)
            {
                // Se crea una lista dentro de un foreach para ir reciviendo la informacion
                // de cada foto entrante y almacenarla en un arreglo para luego analizarla
                var emoface = new EmoFace()
                {
                    // Se le agregan propiedades de recuedro a los atributos de EmoFace  
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture,
                };
                // Se Carga una ,lista con los valores agregados a las propiedades de emoface
                emoface.Emotions = ProcessEmotions(emotion.Scores, emoface);
                // Y se van guardando los valores 
                listaFaces.Add(emoface);
            }
            // Retornamos la listaFaces con los valores
            return listaFaces;

        }
        // Se crea el metodo para 
        private ObservableCollection<EmoEmotion> ProcessEmotions(EmotionScores scores, EmoFace emoface)
        {
            // Se instancia una variable lista de EmoEmotion para Almacenar los valores a las emociones
            var emotionList = new ObservableCollection<EmoEmotion>();

            // con GetType se muestro toda la informacion que tiene scores,Con GetProperties Me devuelve todas
            // las propiedades de scores y en los parametros de Getproperties para que no me devuelva todas las
            // las propiedades le especifico cuales quiero traer. El | es un or binario ya que estamos haciendo 
            // intercambio con una enumeracion. El Instance es para que no me devuelva los campos estaticos
            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // Se filtran los valores recividos que sean de tipo float de properties
            var filterProperties = properties.Where(p => p.PropertyType == typeof(float));
            // Se Crea una variable para los valores que no estan definidos para que se almacenen en el
            var emotype = EmoEmotionEnum.Undetermined;
            // Se recorre filterProperties para irle agregando los valores de puntuacion
            foreach (var prop in filterProperties)
            {
                // Se convierte los parametros de la Emumeracion EmoEmotionEnum EN un valor equivalente
                // a un Enumerable y si no los convierte el valor recivido se convierte en 
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emotype))
                    emotype = EmoEmotionEnum.Undetermined;

                // Se instancia EmoEmotion
                var emoEmotion = new EmoEmotion();
                // Se le agregan los valores a los atributos de EmoEmotion
                emoEmotion.Score = (float)prop.GetValue(scores);
                // Se le almacena un valor Indeterminado a emoEmotion.EmotionType
                emoEmotion.EmotionType = emotype;
                // Se le almacenan las propiedades definidas en el metodo anterior 
                emoEmotion.Face = emoface;
                // Se va llenando la lista con los valores almacenados
                emotionList.Add(emoEmotion);
            }
            // Se returno la Lista que se lleno
            return emotionList;
        }
    }
}
