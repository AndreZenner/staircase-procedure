from utils import create_label,format_number
import tkinter as tk
import numpy as np

from statistics import mean

class StatisticalValueSection(tk.Frame):

    
    def __init__(self, root):

        tk.Frame.__init__(self, root.frame, bg = "white")

        row = 1

        column_condition = 0
        column_mean_threshold = 1
        column_min = 2
        column_max = 3
        column_std = 4
        column_N = 5

        # label 
        create_label(frame=self, row=row, column=column_condition, text="Condition Name", type="label")
        create_label(frame=self, row=row, column=column_mean_threshold, text="Mean Threshold", type="label")
        create_label(frame=self, row=row, column=column_min, text="Min", type="label")
        create_label(frame=self, row=row, column=column_max, text="Max", type="label")
        create_label(frame=self, row=row, column=column_std, text="Std", type="label")
        create_label(frame=self, row=row, column=column_N, text="N", type="label")
        row+=1

            # calculate
        for condition in root.df:
            t_list = list(root.df[condition].dropna())

            # CONDITION NAME
            create_label(frame=self, row=row, column=column_condition, text=condition, type="entry")

            # MEAN THRESHOLD
            mean_threshold = mean(t_list)
            create_label(frame=self, row=row, column=column_mean_threshold, text=format_number(mean_threshold), type="entry")

            # MIN
            minimum = min(t_list)
            create_label(frame=self, row=row, column=column_min, text=format_number(minimum), type="entry" )

            # MAX
            maximum = max(t_list)
            create_label(frame=self, row=row, column=column_max, text=format_number(maximum), type="entry" )

            # STD
            std = np.std(t_list)
            create_label(frame=self, row=row, column=column_std, text=format_number(std), type="entry")

            # N
            create_label(frame= self, row=row, column=column_N, text=len(t_list), type="entry")

            row+=1