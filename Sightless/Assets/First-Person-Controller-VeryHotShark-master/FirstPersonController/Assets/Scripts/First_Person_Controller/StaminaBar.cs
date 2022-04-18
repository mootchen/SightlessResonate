using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class StaminaBar : MonoBehaviour
{
    public Slider staminaBar;

    private int maxStamina = 8000;
    private int currentStaimina;
    private bool canrun;
    public static StaminaBar instance;

    private WaitForSeconds regenTick = new WaitForSeconds(0.5f);
    private Coroutine regen;
    private void Awake(){
        instance = this;
    }

    void Start()
    {
       currentStaimina = maxStamina;
       staminaBar.maxValue = maxStamina;
       staminaBar.value = maxStamina;
    }

    // Update is called once per frame
    public void UseStamina(int amount){
       
        if(currentStaimina - amount >= 0)
        {
            canrun = true;
            currentStaimina -= amount;
            staminaBar.value = currentStaimina;

        if(regen != null)
            StopCoroutine(regen);

           regen =  StartCoroutine(RegenStamina());
        }
        else
        {
            canrun = false;
            print("not enuf statmina");
        }
    }
    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(2);

        while(currentStaimina < maxStamina)
        {
            currentStaimina += maxStamina / 10;
            staminaBar.value = currentStaimina;
            yield return regenTick;
        }
        regen = null;
    }
    public bool CanRunStamina(){
    
        //caqn run is set in useStamina method
        return canrun;
    }
    
    

}