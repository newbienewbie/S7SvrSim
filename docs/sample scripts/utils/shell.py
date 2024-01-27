import clr
clr.AddReference('System')
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing.Primitives')
clr.AddReference("System.Drawing.Common")

import System
import System.Windows.Forms as WinForms

form = WinForms.Form()
form.Size = System.Drawing.Size(310,625)

OKButton = WinForms.Button()
OKButton.Location = System.Drawing.Point(155,550) 
OKButton.Size = System.Drawing.Size(75,23) 
OKButton.Text = 'OK' 
OKButton.DialogResult = WinForms.DialogResult.OK
form.AcceptButton = OKButton
form.Controls.Add(OKButton)

label = WinForms.Label()
label.Location = System.Drawing.Point(10,10) 
label.AutoSize = True
Font = System.Drawing.Font("Arial",12, System.Drawing.FontStyle.Bold) ### Formatting text for the label
label.Font = Font
label.Text="RfidUid"
form.Controls.Add(label)


textBox = WinForms.TextBox()
textBox.Location = System.Drawing.Point(10,40) 
textBox.Size = System.Drawing.Size(275,500)
textBox.Multiline = True 
textBox.AcceptsReturn = True 
form.Controls.Add(textBox)


def _accept_input(convert, label_name) :
    label.Text = label_name
    result = form.ShowDialog()
    text = textBox.Text
    return convert(text)

def accept_input_int( label_name) :
    return _accept_input(int,label_name)

def accept_input_float( label_name) :
    return _accept_input(float,label_name)

def accept_input_str( label_name) :
    return _accept_input(str,label_name)

accept_input = accept_input_int

# rfidUid = accept_input('rfid batch')
# WinForms.MessageBox.Show("%s"%rfidUid)
        
def show_message_box (s) :
    WinForms.MessageBox.Show(str(s))