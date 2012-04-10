using System.Collections.ObjectModel;

namespace xkey7.XKey
{
    public class Model
    {
        public Model()
        {

        }

        public Model(int emergancy, int guistate, int traystate)
        {
            Emergancy = emergancy;
            GuiState = guistate;
            TrayState = traystate;
        }

        public string Active { get; set; }
        /// <summary>
        /// 0 - 
        /// 1 - 
        /// 2 - Disc Read Error, reset your console!
        /// </summary>
        public int Emergancy { get; set; }
        /// <summary>
        /// 0 - 
        /// 1 - 
        /// 2 - 
        /// </summary>
        public int GuiState { get; set; }
        /// <summary>
        /// 0 - open
        /// 1 - closed
        /// </summary>
        public int TrayState { get; set; }
        public ObservableCollection<Mount> Games { get; set; }
        public ObservableCollection<Item> About { get; set; }

        public Mount AddMount(string name)
        {
            if (Games == null) Games = new ObservableCollection<Mount>();
            var m = new Mount(name);
            Games.Add(m);
            return m;
        }

        public void AddAbout(string name, string value)
        {
            if (About == null) About = new ObservableCollection<Item>();
            About.Add(new Item(name, value));
        }
    }
    public class Mount
    {
        public Mount()
        {

        }
        public Mount(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public ObservableCollection<Iso> Isos { get; set; }

        public void AddIso(string title, string id, bool active = false)
        {
            if (Isos == null) Isos = new ObservableCollection<Iso>();
            Isos.Add(new Iso(title, id, active));
        }
    }
    public class Iso
    {
        public Iso()
        {

        }

        public Iso(string title, string id, bool active = false)
        {
            Id = id;
            Title = title;
            Active = active;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }
    }
    public class Item
    {
        public Item()
        {

        }

        public Item(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
