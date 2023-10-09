clear;
close all;
T = readtable('G:\DEPeNDS\Paper2_Git\Data_DEPeNDS_2023.xlsx','Sheet','Sheet1'); 
thres=0.5001;
startVal=43; % where Workshop 3 / validation set starts
idxNotNan=find(~isnan(T.AGE_RT) & ~isnan(T.Diff_Gy_));
Xv(:,1)=T.AGE_RT(idxNotNan);
Xv(:,2)=T.Diff_Gy_(idxNotNan);  

yTemp=T.Prob_protonWS1(idxNotNan)/100;
Yv=T.Prob_protonWS1(idxNotNan)/100<(thres);

%only fitting to the first two workshops
[B,DEV,STATS] =glmfit(Xv(1:startVal-1,:),yTemp(1:startVal-1), 'binomial', 'link', 'logit');

%prediction to trainig and validation set
B=[1.401667; -0.067553; 0.268847] % External fit validation parameters
yhat = glmval(B,Xv,'logit');
yhat_Ws1_2=yhat(1:startVal-1);
yhat_Ws3=yhat(startVal:end);

%AUC model performance
[X,Y,T1,AUC_train] = perfcurve(Yv(1:startVal-1),yhat_Ws1_2,'false');
[X2,Y2,T2,AUC_val] = perfcurve(Yv(startVal:end,:),yhat_Ws3,'false');

figure(1);
subplot(1,2,1);
plot(X,Y,'-r','LineWidth', 2)
hold on;
plot(X2,Y2,'-b','LineWidth', 2)
ax = gca;
ax.FontSize = 12;
legend(['Training AUC=',num2str(round(AUC_train,2))],['Validation AUC=',num2str(round(AUC_val,2))], 'FontSize', 14);
xlabel('False positive rate', 'FontSize', 16); 
ylabel('True positive rate', 'FontSize', 16);
ylim([0 1.05]);
xlim([-0.05 1.0]);
title('ROC for Classification by Logistic Regression', 'FontSize', 18)


%Calibration plot
subplot(1,2,2);
plot(yTemp(1:startVal-1),yhat_Ws1_2,'xr', 'MarkerSize',12,'LineWidth', 2);
hold on;
plot(yTemp(startVal:end),yhat_Ws3,'ob','MarkerSize',12,'LineWidth', 2);
hold on;
h1=plot([-0.10 1.1],[0.5 0.5],'-k');
hold on;
h2=plot([0.5 0.5],[0 1],'-k');

box02=[0 0 0.25 0.25];
boxMiddle=[0.75 0.75 1 1];
box08=[0.25 0.25 0.75 0.75];
boxX=[-0.10 1.1 1.1 -0.10];

patch(boxX,box02,[0 1 0],'FaceAlpha',0.2);
patch(boxX,box08,[0 0 1],'FaceAlpha',0.2);
patch(boxX,boxMiddle,[0 1 0],'FaceAlpha',0.2);
ax = gca;
ax.FontSize = 12; 
xlim([-0.1 1.1])
ylim([0 1])
legend('Training','Validation', 'FontSize', 14);
xlabel('Workshop results', 'FontSize', 16); 
ylabel('Model results', 'FontSize', 16);
title('Calibration plot', 'FontSize', 18);
