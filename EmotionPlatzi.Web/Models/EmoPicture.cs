using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmotionPlatzi.Web.Models
{
    public class EmoPicture
    {
        public int Id { get; set; }
        [Display(Name="Nombre")]
        public string Name { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage ="La Ruta Supera el Tamaño Establecido")]
        public string Path { get; set; }

        public virtual ObservableCollection<EmoFace> Faces{ get; set; }
    }
}