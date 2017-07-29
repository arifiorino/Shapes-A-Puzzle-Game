using UnityEngine;
using System.Collections;

public class Scheduler{
	ArrayList updateFunctionQueue;
	ArrayList initFunctionQueue;
	ArrayList times;
	float elapsedTime=0f;
	float lastTime=0;

	public Scheduler(){
		updateFunctionQueue= new ArrayList();
		initFunctionQueue= new ArrayList();
		times= new ArrayList();
	}

	public void addTime(float time){
		initFunctionQueue.Add((System.Action)delegate{});
		updateFunctionQueue.Add("Nothing");
		times.Add (time);
	}

	public void addInitFunction(System.Action initFunction){
		initFunctionQueue.Add(initFunction);
		updateFunctionQueue.Add("Nothing");
		times.Add (0f);
	}

	public void addInitFunctionWithTime(System.Action initFunction, float time){
		initFunctionQueue.Add(initFunction);
		updateFunctionQueue.Add("Nothing");
		times.Add (time);
	}

	public void addUpdateFunction(System.Func<bool> updateFunction){
		initFunctionQueue.Add((System.Action)delegate{});
		updateFunctionQueue.Add(updateFunction);
		times.Add (-1f);
	}

	public void addUpdateFunction(System.Action initFunction, System.Func<bool> updateFunction){
		initFunctionQueue.Add(initFunction);
		updateFunctionQueue.Add(updateFunction);
		times.Add (-1f);
	}

	public void start(){
		lastTime = Time.time;
	}

	void moveQueue(){
		initFunctionQueue.RemoveAt(0);
		updateFunctionQueue.RemoveAt(0);
		times.RemoveAt (0);
		elapsedTime = 0f;
		lastTime = Time.time;
	}

	public void update(){
		if (times.Count > 0) {
			if (elapsedTime == 0f)
				((System.Action)initFunctionQueue[0])();
			elapsedTime=Time.time-lastTime;
			if ((float)times [0] >= 0f){
				if (elapsedTime >= (float)times[0])
					moveQueue();
			}else{
				if (!((System.Func<bool>)updateFunctionQueue[0])())
					moveQueue();
			}
		}
	}
}
