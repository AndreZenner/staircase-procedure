import time
import sys
import traceback
import logging
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import matplotlib.patches as mpatches
import matplotlib.lines as mlines
from matplotlib.ticker import MaxNLocator
import matplotlib.ticker as ticker
import os


# Set legend layout
def set_legend_layout(threshold):
    red_patch = mpatches.Patch(color='red', label='Sequence1')
    blue_patch = mpatches.Patch(color='blue', label='Sequence2')
    green_patch = mlines.Line2D([], [], color='green', linewidth = 0.8, linestyle = '--',dashes=(2, 3), label='Threshold: ' + str(threshold))
    plus_patch = mlines.Line2D([], [], color='black', marker='+', markersize=6, mew=1, label='Stimulus noticed', linestyle='None')
    minus_patch = mlines.Line2D([], [], color='black', marker='_',markersize=6, mew=1, label='Stimulus not noticed', linestyle='None')
    reversal = mlines.Line2D([], [], color='white',markeredgecolor='black', marker="o", markersize=8, label='Reversal', linestyle='None')
    axs1.legend(handles=[red_patch, blue_patch, plus_patch, minus_patch, reversal, green_patch], facecolor='white', frameon=True, bbox_to_anchor=(1.05, 1.0), loc='upper left', fontsize=6)

# Set layout of the graph like title, axes labels
def set_graph_layout(min_stimulus,max_stimulus, stop_criterion_reversals,stop_amount, plot_window_title):
    axs1.set_title(plot_window_title)
    axs_list = [axs1,axs2,axs3] if show_subplots else [axs1]
    for axs in axs_list:
        axs.margins(y=0.3)   # prevents markers on the edges
        axs.tick_params(axis='x', labelsize=6)  
        axs.tick_params(axis='y', labelsize=6)
        if( stop_criterion_reversals == "True"):
            loc = ticker.MultipleLocator(1)
            axs.xaxis.set_major_locator(loc)
        ymin = min_stimulus-0.2*(max_stimulus-min_stimulus)
        ymax = max_stimulus+0.2*(max_stimulus-min_stimulus)
        axs.set_ylim((ymin, ymax))
        axs.set_ylabel('Stimulus Intensity ', fontsize=6)
    if(stop_criterion_reversals == "False"):
        axs1.set_xticks(np.arange(0, int(stop_amount)+1, 1.0))
        if show_subplots:
            axs2.set_xticks(np.arange(0, int(stop_amount/2)+1, 1.0))
            axs3.set_xticks(np.arange(0, int(stop_amount/2)+1, 1.0))

    axs1.set_xlabel('Trial Number', fontsize=6)
    if show_subplots:
        axs2.set_xlabel('Sequence Index', fontsize=6)
        axs3.set_xlabel('Sequence Index', fontsize=6)
        axs2.title.set_text('Sequence 1')
        axs3.title.set_text('Sequence 2')
    plt.tight_layout()


# create axes and initialize plot
def init(msg):
    global results_path,plot_window_title, axs1,axs2,axs3,show_subplots, experimentName, conditionName, numberParticipant
    min_stimulus,max_stimulus,number_of_steps,stop_criterion_reversals,stop_amount,plot_window_title,results_path, experimentName, conditionName, numberParticipant, show_subplotsString = msg.split(";")
    min_stimulus = float(min_stimulus)
    max_stimulus = float(max_stimulus)
    stop_amount = int(stop_amount)

    if show_subplotsString == "False":
        show_subplots=False
        axs1 = fig.add_subplot(1,1,1)
        fig.set_size_inches(8,2)
    else:
        show_subplots=True
        axs1 = fig.add_subplot(3,1,1)
        axs2 = fig.add_subplot(3,1,2)
        axs3 = fig.add_subplot(3,1,3)

    # switch min and max (in case the experiment uses inverse direction) for axes in ascending numbering
    if(max_stimulus < min_stimulus): 
        v = max_stimulus
        max_stimulus = min_stimulus
        min_stimulus = v

    set_legend_layout(0)
    set_graph_layout(min_stimulus,max_stimulus, stop_criterion_reversals,stop_amount, plot_window_title)
     

# read trial data and plot in graph
def add_trial(msg):
    index_trial,sequence, stimulus, stimulus_noticed, index_sequence, reversal = msg.split(";")
    sequence = int(sequence)
    index_trial = float(index_trial)
    stimulus = float(stimulus)
    index_sequence = int(index_sequence)

    if(stimulus_noticed == "True" and reversal == "True"):
        marker_n = '$\\oplus$'
        mew_n = 0.1
        markersize_n = 10
    elif (stimulus_noticed == "True"):
        marker_n = "+"
        mew_n = 1
        markersize_n = 8
    elif(stimulus_noticed == "False" and reversal == "True"):
        marker_n = '$\\ominus$'
        mew_n = 0.1
        markersize_n = 10
    else:
        marker_n = "_"
        mew_n = 1
        markersize_n = 8

    if(sequence == 1):
        axs1.plot(index_trial, stimulus, marker=marker_n, mew=mew_n, markersize= markersize_n, color='red', label='Sequence1')
        if show_subplots: axs2.plot(index_sequence, stimulus, marker=marker_n, mew=mew_n, markersize= markersize_n, color='red')

    else:
        axs1.plot(index_trial, stimulus, marker=marker_n, mew=mew_n, markersize= markersize_n, color='blue')
        if show_subplots: axs3.plot(index_sequence, stimulus, marker=marker_n, mew=mew_n, markersize= markersize_n, color='blue')

# plot threshold
def plot_threshold(threshold):
    threshold = float(threshold)
    axs1.axhline(threshold, 0, 1, color='green', linewidth = 0.8, linestyle = '--',dashes=(2, 3), zorder=1)
    set_legend_layout(threshold)

# parse messages         
def animate(i):
    if(len(data_array) > 0):
        message = data_array.pop(0)
        arg, msg = message.split("::")
        if arg == "init": 
            init(msg) 
        elif arg == "threshold":
            plot_threshold(msg)
            save_plot()
        elif arg == "trial":
            add_trial(msg)
            save_plot()
        else:
            print("Unknown argument:" + arg)   


# save messages in a list
def append_msg(msg):
    data_array.append(msg)

def create_plot():
    global fig, data_array
    fig = plt.figure()
    data_array=[]

def start_animate():
    try:
        ani = animation.FuncAnimation(fig, animate, interval=2000)
    except Exception as e:
        logging.error(traceback.format_exc())

    plt.show()

def show_plot():
    plt.show()

def save_plot():
    dirname = "P_" + experimentName + "_" + numberParticipant;
    filename = "P_" + experimentName + "_" + numberParticipant + "_" + conditionName + "_plot.svg";
    path = os.path.join(results_path, experimentName, dirname, filename)
    plt.savefig(path, format="svg", transparent=True)

def fileplotter_save_plot(path):
    plt.savefig(path, format="svg", transparent=True)
