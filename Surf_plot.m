close all
T = readtable('G:\DEPeNDS\Paper2_Git\Data_DEPeNDS_2023.xlsx','Sheet','Sheet1'); 
thres=0.5001;
idxNotNan=find(~isnan(T.AGE_RT) & ~isnan(T.Diff_Gy_));
Xv(:,1)=T.AGE_RT(idxNotNan);
Xv(:,2)=T.Diff_Gy_(idxNotNan);  

yTemp=T.Prob_protonWS1(idxNotNan)/100;
Yv=T.Prob_protonWS1(idxNotNan)/100<(thres);
startVal=43;

[B,DEV,STATS] =glmfit(Xv(1:startVal-1,:),yTemp(1:startVal-1), 'binomial', 'link', 'logit');
[B_prob,DEV_prob,STATS_prob] =glmfit(Xv(1:startVal-1,:),yTemp(1:startVal-1), 'binomial', 'link', 'probit');

[X1,X2] = meshgrid(min(Xv(:,1)):2:max(Xv(:,1)),min(Xv(:,2)):2:max(Xv(:,2)));
yhat = glmval(B,[X1(:) X2(:)],'logit');
yhat =reshape(yhat,size(X1));

B=[1.401667; -0.067553; 0.268847];% External fit validation parameters from R
yhat_CRH = glmval(B,[X1(:) X2(:)],'logit');
yhat_CRH =reshape(yhat_CRH,size(X1));

figure;
%surf(X1,X2,yhat,'FaceAlpha',0.4)
%hold on;
surf(X1,X2,yhat_CRH,'FaceAlpha',0.4)
hold on;
plot3(Xv(1:startVal-1,1),Xv(1:startVal-1,2),yTemp(1:startVal-1),'or','MarkerFaceColor',[1 0 0]);
hold on;
plot3(Xv(startVal:end,1),Xv(startVal:end,2),yTemp(startVal:end),'ob','MarkerFaceColor',[0 0 1]);
xlabel('Age @ RT');
ylabel('\Delta mean dose [Gy] (Brain-CTV-BS) ');
zlabel('Fraction of clinicians choosing proton therapy');
title('Decision surface plot');
yhat_kryds = glmval(B,[40 10],'logit');
hold on; plot3([40],[10],[yhat_kryds],'xk','MarkerSize',12, 'LineWidth', 4);
hold on; plot3([40 40],[10 10],[0 yhat_kryds],'-k');
hold on; plot3([40 40],[0 10],[0 0],'-k');
hold on; plot3([20 40],[10 10],[0 0],'-k');
legend('model prediction','training data', 'validation data','example data');
