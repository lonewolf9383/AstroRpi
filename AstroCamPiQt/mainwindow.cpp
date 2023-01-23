#include "mainwindow.h"
#include "./ui_mainwindow.h"
#include "../AstroCamLib/ImageStacker.h"
#include <QFileDialog>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}


void MainWindow::on_actionExit_triggered()
{
    QApplication::quit();
}


void MainWindow::on_actionStack_triggered()
{
    QStringList selectedFiles = QFileDialog::getOpenFileNames(this, tr("Image Files"), nullptr, tr("Images (*.png *.xpm *.jpg);"), nullptr);
    std::vector<AstroCamLib::ImagePtr> loadedImages;
    loadedImages.reserve(selectedFiles.size());
    for (QString s : selectedFiles)
    {
        loadedImages.push_back(AstroCamLib::Image::FromFile(s.toStdString()));
    }

    
    auto ptr = loadedImages.begin();
    AstroCamLib::ImageStacker stacker(*ptr);
    ++ptr;
    for (; ptr != loadedImages.end(); ++ptr)
    {
        stacker.AlignImage(*ptr);
    }

}

