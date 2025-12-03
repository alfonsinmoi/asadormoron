using System.ComponentModel;
using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class CategoriasArticulosGroupModel : List<Comida>, INotifyPropertyChanged
    {
        private bool _expanded;
        public string Categoria { get; set; }
        public string Categoria_eng { get; set; }
        public string Categoria_fr { get; set; }
        public string Categoria_ger { get; set; }
        public string ColorCategoria { get; set; }
        public string TitleWithItemCount
        {
            get { return $"{Categoria} ({EventosCount})"; }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(nameof(Expanded));
                    OnPropertyChanged(nameof(StateIcon));
                }
            }
        }

        public string StateIcon
        {
            get { return Expanded ? "collapsed.png" : "expanded.png"; }
        }

        public int EventosCount { get; set; }

        public CategoriasArticulosGroupModel(string title, string title_eng, string title_ger, string title_fr, bool expanded = true, string color = "")
        {
            Categoria = title;
            Categoria_eng = title_eng;
            Categoria_fr = title_fr;
            Categoria_ger = title_ger;
            ColorCategoria = color;
            Expanded = expanded;
        }
#pragma warning disable CS0114 // 'CursosGroup.PropertyChanged' oculta el miembro heredado 'List<CursosModel>.PropertyChanged'. Para hacer que el miembro actual invalide esa implementación, agregue la palabra clave override. Si no, agregue la palabra clave new.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0114 // 'CursosGroup.PropertyChanged' oculta el miembro heredado 'List<CursosModel>.PropertyChanged'. Para hacer que el miembro actual invalide esa implementación, agregue la palabra clave override. Si no, agregue la palabra clave new.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
