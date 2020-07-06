# Getting Started Guide

## Installation
1. Follow the [ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Installation.md) installatioin guide.

## RollerBall Procedure
1. Launch Unity Hub.
2. Choose **Add** option on Project dialog
3. Locate RollerBall Project in 'UnityEnvModelingFactorySetup/RollerBall' and click **Open**.
4. Drag **SampleScene** located in 'Scene' directory to **Hierarcy**
5. Start modify!

## ML-Agents Procedure
1. Launch RollerBall
2. Open Terminal. 
3. Activate **ml-agents** environment.
4. Run 'mlagents-learn PATH/To/Your/Environment/UnityEnvModellingFactorySetup/RollerBall/config/ppo/rollerball_config.yaml --run-id=RollerBall_Run1'.
5. Once 'Start training by pressing the Play button in the Unity Editor' shows up on your terminal, press **Play** button in Unity.
6. **Mean Reward** will be updated on Terminal.
7. Press **Play** button in Unity to end training process.
8. To check the result, type 'tensorboard --logdir=results' on Terminal.
9. For even in depth explanation, check out Unity's ML-Agents [Getting Started Guide](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Getting-Started.md).

