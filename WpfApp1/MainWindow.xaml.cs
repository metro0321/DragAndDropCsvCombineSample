using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public ObservableCollection<ViewFilePath> fpathCollection;

        public class ViewFilePath
		{
            public string text { get; set; }
		}

        public MainWindow()
		{
            fpathCollection = new ObservableCollection<ViewFilePath> {};

            InitializeComponent();

			EnableDragDrop(filePathDataGrid);

            filePathDataGrid.CanUserAddRows = false;
            filePathDataGrid.CanUserDeleteRows = false;
            filePathDataGrid.CanUserResizeRows = false;
            filePathDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            filePathDataGrid.RowHeaderWidth = 15;
        }

        private void EnableDragDrop(Control control)
        {
            //ドラッグ＆ドロップを受け付けられるようにする
            control.AllowDrop = true;

            //ドラッグが開始された時のイベント処理（マウスカーソルをドラッグ中のアイコンに変更）
            control.PreviewDragOver += (s, e) =>
            {
                //ファイルがドラッグされたとき、カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
                e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
                e.Handled = true;
            };

            //ドラッグ＆ドロップが完了した時の処理（ファイル名を取得し、ファイルの中身をTextプロパティに代入）
            control.PreviewDrop += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) // ドロップされたものがファイルかどうか確認する。
                {
                    string[] paths = ((string[])e.Data.GetData(DataFormats.FileDrop));
                    //--------------------------------------------------------------------
                    // ここに、ドラッグ＆ドロップ受付時の処理を記述する
                    //--------------------------------------------------------------------
                    foreach(string p in paths)
					{
                        ViewFilePath vfp = new ViewFilePath();
                        vfp.text = p;
                        fpathCollection.Add(vfp);
                    }
                    filePathDataGrid.ItemsSource = fpathCollection;
                }
            };

        }

		private void runButton_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                string combine_string = "Work Item Type,Title,Priority,Tags,Repro Steps\r\n";
                // Open the text file using a stream reader.
                foreach (ViewFilePath p in fpathCollection)
                {
                    string path = p.text;
                    using (var sr = new StreamReader(path))
                    {
                        // Read the stream as a string, and write the string to the console.
                        string line;
                        int count = 0;
                        while((line = sr.ReadLine()) != null)
						{
                            if(count != 0)
							{
                                combine_string += line;
                                combine_string += "\r\n";
                            }
                            count++;
						}
                        
                    }
                }

                var output = new StreamWriter("CombineIssues.csv");
                output.Write(combine_string);
                output.Flush();
                output.Close();
            }
            catch (IOException exp)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(exp.Message);
            }
        }
	}
}
