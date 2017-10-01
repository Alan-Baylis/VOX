/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using System;

namespace VOX{
	public class OnlyOnDate : MonoBehaviour{
		
		public GameObject target;

		[Header("Date")]
		public bool reverse;
		[Range(1, 31)] public int day;
		[Range(1, 12)] public int month;
		public int year;

		void OnEnable(){
			Refresh();
		}

		public void Refresh(){
			DateTime today = DateTime.Today;
			
			bool isDay = day > 0 ? (day == today.Day) : true;
			bool isMonth = month > 0 ? (month == today.Month) : true;
			bool isYear = year > 0 ? (year == today.Year) : true;
			bool isToday = isDay && isMonth && isYear;

			bool isActive = reverse ? !isToday : isToday;
			target.SetActive(isActive);
		}
	}
}