using UnityEngine;

public class VaultHandler : MonoBehaviour {
	private UIHandler uiHandler;

	private float powerStored, foodStored, waterStored;
	private float powerIn, powerOut, foodIn, foodOut, waterIn, waterOut;
	private Producer[] allProducers; // All GameObjects with a Producer script attached
	private Storage[] allStorage; // All GameObjects with a Storage script attached

	private int updateTimer;

	void Start() {
		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();

		powerStored = 0f;
		foodStored = 0f;
		waterStored = 0f;

		powerIn = 0f;
		powerOut = 0f;

		foodIn = 0f;
		foodOut = 0f;

		waterIn = 0f;
		waterOut = 0f;

		updateTimer = 0;
	}

	void Update() {
		if (updateTimer == Application.targetFrameRate) {
			allProducers = GameObject.FindObjectsByType<Producer>();
			allStorage = GameObject.FindObjectsByType<Storage>();

			float maxPowerStored = UpdatePowerStats();
			float maxFoodStored = UpdateFoodStats();
			float maxWaterStored = UpdateWaterStats();

			uiHandler.DisplayStoredAndIO(powerStored, maxPowerStored, powerIn, powerOut, foodStored, maxFoodStored, waterStored, maxWaterStored);

			updateTimer = 0;
		} else {
			++updateTimer;
		}
	}

	private float UpdatePowerStats() {
		powerIn = 0f;
		powerOut = 0f;

		foreach (Producer producer in allProducers) {
			if (producer.powerProduction >= 0f) {
				powerIn += producer.powerProduction;
			} else {
				powerOut += producer.powerProduction;
			}
		}

		float maxPowerStored = 0f;
		foreach (Storage s in allStorage) {
			if (s.maxPower > 0) {
				maxPowerStored += s.maxPower;
			}
		}

		float netPower = powerIn + powerOut;
		powerStored = Mathf.Max(0f, Mathf.Min(powerStored + netPower, maxPowerStored));

		float powerToDistribute = powerStored;
		foreach (Storage s in allStorage) {
			if (s.maxPower > 0) {
				float difference = s.maxPower - s.powerStored;

				if (difference > powerToDistribute) {
					s.powerStored += powerToDistribute;
					break;
				} else {
					s.powerStored = s.maxPower;
					powerToDistribute -= difference;
				}
			}
		}

		return maxPowerStored;
	}

	private float UpdateFoodStats() {
		foodIn = 0f;
		foodOut = 0f;

		foreach (Producer producer in allProducers) {
			if (producer.foodProduction >= 0f) {
				foodIn += producer.foodProduction;
			} else {
				foodOut += producer.foodProduction;
			}
		}

		float maxFoodStored = 0f;
		foreach (Storage s in allStorage) {
			if (s.maxFood > 0) {
				maxFoodStored += s.maxFood;
			}
		}

		float netFood = foodIn + foodOut;
		foodStored = Mathf.Max(0f, Mathf.Min(foodStored + netFood, maxFoodStored));

		float foodToDistribute = foodStored;
		foreach (Storage s in allStorage) {
			if (s.maxFood > 0) {
				float difference = s.maxFood - s.foodStored;

				if (difference > foodToDistribute) {
					s.foodStored += foodToDistribute;
					break;
				} else {
					s.foodStored = s.maxFood;
					foodToDistribute -= difference;
				}
			}
		}

		return maxFoodStored;
	}

	private float UpdateWaterStats() {
		waterIn = 0f;
		waterOut = 0f;

		foreach (Producer producer in allProducers) {
			if (producer.waterProduction >= 0f) {
				waterIn += producer.waterProduction;
			} else {
				waterOut += producer.waterProduction;
			}
		}

		float maxWaterStored = 0f;
		foreach (Storage s in allStorage) {
			if (s.maxWater > 0) {
				maxWaterStored += s.maxWater;
			}
		}

		float netWater = waterIn + waterOut;
		waterStored = Mathf.Max(0f, Mathf.Min(waterStored + netWater, maxWaterStored));

		float waterToDistribute = waterStored;
		foreach (Storage s in allStorage) {
			if (s.maxWater > 0) {
				float difference = s.maxWater - s.waterStored;

				if (difference > waterToDistribute) {
					s.waterStored += waterToDistribute;
					break;
				} else {
					s.waterStored = s.maxWater;
					waterToDistribute -= difference;
				}
			}
		}

		return maxWaterStored;
	}
}
