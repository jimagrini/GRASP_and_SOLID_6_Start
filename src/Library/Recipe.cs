//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent // Modificado por DIP
    {
        private class TimerAdapter : TimerClient
        {
            private Recipe recipe;
            public TimerAdapter(Recipe recipe)
            {
                this.recipe = recipe;
            }

            public object TimeOutId { get; }

            public void TimeOut()
            {
                this.recipe.cooked = true;
            }
        }

        private TimerAdapter timerClient;
        private CountdownTimer timer = new CountdownTimer();


        // Cambiado por OCP

        public bool cooked { get; private set; } = false;

        public bool Cooked
        {
            get
            {
                return this.cooked;
            }
        }

        private IList<BaseStep> steps = new List<BaseStep>();

        public Product FinalProduct { get; set; }

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        private void StartCountdown()
        {
            this.timerClient = new TimerAdapter(this);
            this.timer.Register(GetCookTime(), this.timerClient);
        }

        public void Cook()
        {
            if(this.cooked==true)
            {
                InvalidOperationException exc = new InvalidOperationException("La receta ya esta cocinada");
                throw exc;
            }
            this.StartCountdown();
        }
        public int GetCookTime()
        {
            int recipeTime = 0;
            foreach (BaseStep step in this.steps)
            {
                recipeTime += step.Time;
            }
            return recipeTime;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }
    }
}



