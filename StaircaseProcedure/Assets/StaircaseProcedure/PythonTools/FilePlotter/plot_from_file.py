import sys
import csv
import threading
from threading import Thread

# import script
import os.path
direc = (os.path.abspath(os.path.join(os.path.dirname(__file__), '..')) + '/LivePlotter/')
sys.path.append(direc)
import plotter

window_width = 12 #size of plot/window in inches
window_height = 6

delim = sys.argv[1]
print("\nusing delimiter: ")
print(delim)
file_path =  sys.argv[2]
print("\nusing file_path: ")
print(file_path)
if len(sys.argv) >= 4:
    save_svg_path =  sys.argv[3]
    print("\nusing save_svg_path: ")
    print(save_svg_path)
if len(sys.argv) >= 5:
    window_width =  int(sys.argv[4])
    print("\nusing window_width: ")
    print(window_width)
if len(sys.argv) >= 6:
    window_height =  int(sys.argv[5])
    print("\nusing window_height: ")
    print(window_height)

# parse file and pass data to plotter
def read_file():
    with open(file_path, 'r') as file:
        reader = csv.reader(file, delimiter=delim)
        for index, row_csv in enumerate(reader):
            if row_csv:
                if row_csv[0] == "name":
                    experiment_name = row_csv[1]
                    condition_name = row_csv[2]
                    number_participant = row_csv[3]
                    plotTitle = row_csv[4]
                    if plotTitle == "":
                        plotTitle = "Staircase Results - " + row_csv[2] + " - Participant " + row_csv[3];
                    print("\nusing plotTitle: ")
                    print(plotTitle)

                elif row_csv[0] == "init":  
                    init_array = [row_csv[1],row_csv[2],row_csv[3],row_csv[8],row_csv[6],plotTitle,"",experiment_name,condition_name,number_participant, row_csv[10]]
                    init_string = ';'.join(init_array)
                    plotter.init(init_string)

                elif row_csv[0] == "trial":
                    trial_array = [row_csv[1],row_csv[2],row_csv[3],row_csv[4],row_csv[5],row_csv[6]]
                    data_string = ';'.join(trial_array)
                    plotter.add_trial(data_string)

                elif row_csv[0] == "threshold":
                    plotter.plot_threshold(row_csv[1])

                else: 
                    pass
            else:
                pass


plotter.create_plot_with_size(window_width, window_height)
read_file()
if len(sys.argv) >= 4: plotter.fileplotter_save_plot(save_svg_path)
plotter.show_plot()