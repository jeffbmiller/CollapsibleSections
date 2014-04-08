using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.Linq;

namespace CollapsibleSections
{
    public class MainViewController : DialogViewController
    {
        public MainViewController():base(new RootElement("Collapsable Sections"))
        {
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            NSNotificationCenter.DefaultCenter.AddObserver("SectionTapped", delegate {
                Root.TableView.ReloadData();
           });
         
            TableSource = new MySource(this);
        }

        public Source TableSource { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Root.Add(new CollapsibleSection("Cities", false){ new StringElement("Brandon"), new StringElement("Winnipeg"), new StringElement("Portage") });
            Root.Add(new CollapsibleSection("Provinces",false){ new StringElement("MB"), new StringElement("AB"), new StringElement("SK") });
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return TableSource;
        }
    }

    public class MySource : DialogViewController.Source
    {
        public MySource(DialogViewController dvc):base(dvc)
        {
            
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            var rows = base.RowsInSection(tableview, section);
            var parentSection = Root[section] as CollapsibleSection;
            if (parentSection.IsExpanded)
                return rows;
            return 0;
        }
    }

    public class CollapsibleSection : Section
    {
        private readonly string cellId = "collapsibleCell";
        private bool isCollapsed;
        private readonly UIImageView imageView;
        private UILabel titleLabel;
        public CollapsibleSection (string caption, bool isCollapsed):base(caption)
        {
            this.isCollapsed = isCollapsed;
            imageView = new UIImageView(new RectangleF(300,0,20,20));
            imageView.Image = isCollapsed ? UIImage.FromBundle("chevrondown.png") : UIImage.FromBundle("chevronup.png");
            HeaderView = new UIView(new RectangleF(0,0,200,20));
            titleLabel = new UILabel(new RectangleF(5,0,200,20));
            titleLabel.BackgroundColor = UIColor.Clear;
            titleLabel.Text = caption;
            HeaderView.AddSubview(titleLabel);
            HeaderView.BackgroundColor = UIColor.LightGray;
            HeaderView.AddSubview(imageView);
            HeaderView.AddGestureRecognizer(new UITapGestureRecognizer(x => SectionTapped() ));
        }

        public bool IsExpanded 
        {
            get { return isCollapsed; } 
            set {isCollapsed = value;}
        }

        private void SectionTapped()
        {
            Console.WriteLine("{0} Section Tapped",Caption);
            isCollapsed = !isCollapsed;
            imageView.Image = isCollapsed ? UIImage.FromBundle("chevrondown.png") : UIImage.FromBundle("chevronup.png");
            NSNotificationCenter.DefaultCenter.PostNotificationName("SectionTapped",titleLabel);
        }
    }

}

