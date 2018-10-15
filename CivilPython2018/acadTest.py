import clr

path = 'C:\\Program Files\\Autodesk\\AutoCAD 2018\\'

clr.AddReferenceToFileAndPath(path + 'acdbmgd.dll')

clr.AddReferenceToFileAndPath(path + 'acmgd.dll')

clr.AddReferenceToFileAndPath(path + 'accoremgd.dll')
clr.AddReferenceToFileAndPath(path + 'acdbmgdbrep.dll')
 

import Autodesk

import Autodesk.AutoCAD.Runtime as ar

import Autodesk.AutoCAD.ApplicationServices as aas

import Autodesk.AutoCAD.DatabaseServices as ads

import Autodesk.AutoCAD.Geometry as ag

import Autodesk.AutoCAD.Internal as ai

from Autodesk.AutoCAD.Internal import Utils

 

# Function to register AutoCAD commands

# To be used via a function decorator

 

def autocad_command(function):

 

    # First query the function name

    n = function.__name__

 

    # Create the callback and add the command

    cc = ai.CommandCallback(function)

    Utils.AddCommand('pycmds', n, n, ar.CommandFlags.Modal, cc)

 

    # Let's now write a message to the command-line

    doc = aas.Application.DocumentManager.MdiActiveDocument

    ed = doc.Editor

    ed.WriteMessage("\nRegistered Python command: {0}", n)

 

# A simple "Hello World!" command

 

@autocad_command

def msg():

    doc = aas.Application.DocumentManager.MdiActiveDocument

    ed = doc.Editor

    ed.WriteMessage("\nOur test command works!")

 

# And one to do something a little more complex...

# Adds a circle to the current space

 

@autocad_command

def mycir():

 

    doc = aas.Application.DocumentManager.MdiActiveDocument

    db = doc.Database

 

    tr = doc.TransactionManager.StartTransaction()

    bt = tr.GetObject(db.BlockTableId, ads.OpenMode.ForRead)

    btr = tr.GetObject(db.CurrentSpaceId, ads.OpenMode.ForWrite)

 

    cir = ads.Circle(ag.Point3d(10,10,0),ag.Vector3d.ZAxis, 2)

 

    btr.AppendEntity(cir)

    tr.AddNewlyCreatedDBObject(cir, True)

 

    tr.Commit()

    tr.Dispose()