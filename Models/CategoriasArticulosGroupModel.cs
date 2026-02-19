using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AsadorMoron.Models
{
    public class CategoriasArticulosGroupModel : ObservableCollection<Comida>, INotifyPropertyChanged
    {
        private bool _expanded;
        private int _eventosCount;
        private static readonly List<Comida> _empty = new List<Comida>();

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
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Expanded)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(StateIcon)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(VisibleItems)));
                }
            }
        }

        /// <summary>
        /// Items para el BindableLayout: devuelve la colección si expandido, vacía si colapsado.
        /// Así no se crean views para grupos colapsados (carga lazy).
        /// </summary>
        public IEnumerable<Comida> VisibleItems
        {
            get { return _expanded ? this : _empty; }
        }

        public string StateIcon
        {
            get { return Expanded ? "expanded.png" : "collapsed.png"; }
        }

        public int EventosCount
        {
            get { return _eventosCount; }
            set
            {
                if (_eventosCount != value)
                {
                    _eventosCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(EventosCount)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(TitleWithItemCount)));
                }
            }
        }

        public void AddItem(Comida item)
        {
            Add(item);
        }

        public void RemoveItem(int idArticulo)
        {
            var item = this.FirstOrDefault(a => a.articulo.idArticulo == idArticulo);
            if (item != null)
                Remove(item);
        }

        public CategoriasArticulosGroupModel(string title, string title_eng, string title_ger, string title_fr, bool expanded = true, string color = "")
        {
            Categoria = title;
            Categoria_eng = title_eng;
            Categoria_fr = title_fr;
            Categoria_ger = title_ger;
            ColorCategoria = color;
            _expanded = expanded;
        }
    }
}
