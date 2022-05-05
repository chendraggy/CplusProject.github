#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    this->setCentralWidget(ui->textEdit);
}

MainWindow::~MainWindow()
{
    delete ui;
}

//New事件
void MainWindow::on_actionNew_triggered()
{
    currentFile.clear();        //清除最近的檔案
    ui->textEdit->setText(QString()); //文字框清空
}

//開啟檔案事件
void MainWindow::on_actionOpen_2_triggered()
{
    //讀取檔案函數
    QString fileName = QFileDialog::getOpenFileName(this,"Open the file");
    QFile file(fileName);
    currentFile=fileName;
    //如果檔案無法讀取
    if(!file.open(QIODevice::ReadOnly|QFile::Text)){
        QMessageBox::warning(this,"Warning","Cannot open file : "+file.errorString());
        return;
    }
    //設置Title為讀取的檔名
    setWindowTitle(fileName);
    //文字存取的Stream，讀取File的reference
    QTextStream in(&file);
    //將文字存取Stream匯入QString裡然後交給UI去設置到textEdit
    QString text = in.readAll();
    ui->textEdit->setText(text);
    file.close();
}

//儲存檔案事件
void MainWindow::on_actionSave_as_triggered()
{
    //存取檔案函數
    QString fileName=QFileDialog::getSaveFileName(this,"Save as");
    QFile file(fileName);
    //如果檔案無法儲存
    if(!file.open(QFile::WriteOnly|QFile::Text)){
        QMessageBox::warning(this,"Warning","Cannot save file : "+file.errorString());
        return;
    }
    //更新系統讀取到的檔案名稱
    currentFile=fileName;
    setWindowTitle(fileName);
    //文字存取的Stream，讀取File的reference
    QTextStream out(&file);
    //抓取程式中的文字
    QString text=ui->textEdit->toPlainText();
    //輸出到文字存取的Stream裡
    out<<text;
    file.close();
}

//列印事件
void MainWindow::on_actionPrint_triggered()
{
    QPrinter printer;
    printer.setPrinterName("Printer Name");
    QPrintDialog pDialog(&printer,this);
    if(pDialog.exec()==QDialog::Rejected)
    {
        QMessageBox::warning(this,"Warning","Cannot Access printer");
        return;
    }
    ui->textEdit->print(&printer);
}

//退出事件
void MainWindow::on_actionExit_triggered()
{
    QApplication::quit();
}

//複製事件
void MainWindow::on_actionCopy_triggered()
{
    ui->textEdit->copy();
}

//貼上事件
void MainWindow::on_actionPaste_triggered()
{
    ui->textEdit->paste();
}

//剪下事件
void MainWindow::on_actionCut_triggered()
{
    ui->textEdit->cut();
}

//返回上一步事件
void MainWindow::on_actionUndo_triggered()
{
    ui->textEdit->undo();
}

//重新上一步事件
void MainWindow::on_actionRedo_triggered()
{
    ui->textEdit->redo();
}

