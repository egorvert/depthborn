using System;
namespace MagicPigGames
{
    [Serializable]
    public class HorizontalProgressBar : ProgressBar
    {
        public void UpdateOxygenBar(PlayerMovement pm){
			float currentOxygen = pm.currentOxygen / 100;
			
			SetProgress(currentOxygen);
		}
    }
}
