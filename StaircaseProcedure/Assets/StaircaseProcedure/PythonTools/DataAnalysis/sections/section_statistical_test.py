import tkinter as tk
from utils import create_label, format_number
from scipy.stats import wilcoxon, shapiro
from scipy.stats import friedmanchisquare
import itertools

class StatisticalTestSection(tk.Frame):

    def __init__(self, root):

        self.root = root;

        tk.Frame.__init__(self, root.frame, bg = "white")

        n_cond =  len(root.df.columns)

        print(root.df.to_string())


        # Shapiro Wilk Test for Normality
        if(n_cond >= 3):
            create_label(frame=self,text="Normality Test:  Shapiro-Wilk test",type="heading", method="pack")
            self.shapiroTest()

        create_label(frame=self,text="Non-parametric statistical test",type="heading", method="pack")

        create_label(frame=self, text="Number of Conditions: " + str(n_cond), type="bold", method="pack")


        # Wilcoxon Signed Rank Test
        if(n_cond ==2):
            self.wilcoxonTest()
          
        # Friedman Test (+ pairwise Wilcoxon Test)
        if(n_cond >= 3):
            self.friedmanTest(n_cond)


    def shapiroTest(self):
        create_label(frame=self,text="Perform the Shapiro-Wilk test for normality. The Shapiro-Wilk test tests the null hypothesis that the data was drawn from a normal distribution.", type="bold", method="pack")
        
        frame_shapiro_variables = tk.Frame(self, bg = "white")
        frame_shapiro_variables.pack(pady='5',fill="x")


        row = 0

        create_label(frame=frame_shapiro_variables, row=row, column=0, text="" , type="bold")
        create_label(frame=frame_shapiro_variables, row=row, column=1, text="statistic" , type="bold")
        create_label(frame=frame_shapiro_variables, row=row, column=2, text="p" , type="bold")
        create_label(frame=frame_shapiro_variables, row=row, column=3, text="distribution" , type="bold")

        row+=1
        all_normally_distributed = True

        for condition in self.root.df.columns:

            shapiro_test = shapiro(self.root.df[condition])

            create_label(frame=frame_shapiro_variables, row=row, column=0, text=str(condition), type="entry")
            create_label(frame=frame_shapiro_variables, row=row, column=1, text=format_number(shapiro_test.statistic), type="entry")
            create_label(frame=frame_shapiro_variables, row=row, column=2, text=format_number(shapiro_test.pvalue), type="entry")

            if(shapiro_test.pvalue >= 0.05):
                create_label(frame=frame_shapiro_variables, row=row, column=3, text="normally distributed",color="green", type="entry")
            else:
                create_label(frame=frame_shapiro_variables, row=row, column=3, text="not normally distributed",color="red", type="entry")
                all_normally_distributed = False

            row+=1

        if (all_normally_distributed): create_label(frame=self, text="All Conditions were normally distributed." , type="bold", method="pack")
        else: create_label(frame=self, text="Normality can not be assumed. You should procede with non-parametric test." , type="bold", method="pack")

    def wilcoxonTest(self):
        w,p = wilcoxon(self.root.df.iloc[:,0], self.root.df.iloc[:,1])

        create_label(frame=self,text="A Wilcoxon Signed Rank Test was used to compare the thresholds of Condition A and Condition B. " , type="bold", method="pack")
        frame_tests_variables = tk.Frame(self, bg = "white")
        frame_tests_variables.pack(pady='5',fill="x")

        row = 0
        create_label(frame=frame_tests_variables, row=row, column=0, text="w = " , type="normal")
        create_label(frame=frame_tests_variables, row=row, column=1, text=w, type="entry")
        row+=1
        create_label(frame=frame_tests_variables, row=row, column=0, text="p = " , type="normal")
        create_label(frame=frame_tests_variables, row=row, column=1, text=p, type="entry")
        row+=1
        create_label(frame=frame_tests_variables, row=row, column=0, text="\u03B1 = " , type="normal")
        create_label(frame=frame_tests_variables, row=row, column=1, text=0.05, type="entry")

        if (p < 0.05):
            text = "p < 0.05: The difference between the mean thresholds of condition A and condition B is statistically significant."
            create_label(frame=self,text=text,color="green", type="bold", method="pack")
        else:
            text = "p >= 0.05: There is no statistically significant difference between the mean thresholds of condition A and condition B."
            create_label(frame=self,text=text,color="red", type="bold", method="pack")


    def friedmanTest(self,n_cond):
        create_label(frame=self,text="A Friedman-Test is run on all " + str(n_cond) + " conditions. The Friedman test tests the null hypothesis that repeated measurements of the same individuals have the same distribution.", type="bold", method="pack")

        statistic, pvalue = friedmanchisquare(*[self.root.df[column] for column in self.root.df])

        frame_tests_variables = tk.Frame(self, bg = "white")
        frame_tests_variables.pack(pady='5',fill="x")

        row = 0
        create_label(frame=frame_tests_variables, row=row, column=0, text="chi-square = " , type="bold")
        create_label(frame=frame_tests_variables, row=row, column=1, text=statistic, type="entry")
        row+=1
        create_label(frame=frame_tests_variables, row=row, column=0, text="p-Value = " , type="bold")
        create_label(frame=frame_tests_variables, row=row, column=1, text=pvalue, type="entry")
        row+=1
        create_label(frame=frame_tests_variables, row=row, column=0, text="\u03B1 = " , type="bold")
        create_label(frame=frame_tests_variables, row=row, column=1, text=0.05, type="entry")

        if (pvalue < 0.05):
            text = "p < 0.05: The differences between some of the medians are statistically significant. Continuing with pair-wise Wilcoxon-Signed-Rank-Test with Bonferroni Correction."
            create_label(frame=self,text=text,color="green", type="bold", method="pack")
            self.pairwiseWilcoxonTest()

        else:
            text = "p >= 0.05: The differences between the medians are not statistically significant"
            create_label(frame=self,text=text,color="red", type="bold", method="pack")


    def pairwiseWilcoxonTest(self):
        cond_list = list(self.root.df.columns)

        cond_pairs = list(itertools.combinations(cond_list, 2))

        frame_pairwise_wilxocon = tk.Frame(self, bg = "white")
        frame_pairwise_wilxocon.pack(pady='5',fill="x")

        row = 0

        create_label(frame=frame_pairwise_wilxocon, row=row, column=0, text="Pair" , type="bold")
        create_label(frame=frame_pairwise_wilxocon, row=row, column=1, text="w" , type="bold")
        create_label(frame=frame_pairwise_wilxocon, row=row, column=2, text="p" , type="bold")
        create_label(frame=frame_pairwise_wilxocon, row=row, column=3, text="p * #Pairs (= Bonferroni corrected)" , type="bold")

        row+=1

        for pair in cond_pairs:

            w,p = wilcoxon(self.root.df[pair[0]],self.root.df[pair[1]])
            create_label(frame=frame_pairwise_wilxocon, row=row, column=0, text=str(pair[0]) + " vs. " +  str (pair[1]), type="entry")
            create_label(frame=frame_pairwise_wilxocon, row=row, column=1, text=w, type="entry")
            create_label(frame=frame_pairwise_wilxocon, row=row, column=2, text=p, type="entry")
            create_label(frame=frame_pairwise_wilxocon, row=row, column=3, text=p * len(cond_pairs), type="entry")

            if((p * len(cond_pairs)) <0.05):
                create_label(frame=frame_pairwise_wilxocon, row=row, column=4, text="significant",color="green", type="entry")
            else:
                create_label(frame=frame_pairwise_wilxocon, row=row, column=4, text="not significant",color="red", type="entry")

            row+=1
