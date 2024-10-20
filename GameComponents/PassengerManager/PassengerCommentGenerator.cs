using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class PassengerCommentGenerator
{
    static List<Func<PassengerData, PassengerReview, string>> methods = new List<Func<PassengerData, PassengerReview, string>>()
    {
        ReviewA,
        ReviewB,
        ReviewC,
        ReviewD,
        ReviewE,
        ReviewF,
        ReviewG,
        ReviewH,
    };

    static string ReviewA(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"Service was awful. It took forever to reach {passenger.destination.name}, and there was nothing edible on board.";
        }
        else if (review.score == 2)
        {
            return $"Service was bad. It shouldn't take that long to go from {passenger.origin.name} to {passenger.destination.name}.";
        }
        else if (review.score == 3)
        {
            return $"Service was average. I reached {passenger.destination.name} in time.";
        }
        else if (review.score == 4)
        {
            return $"Service was good. I reached {passenger.destination.name} in time, and had some decent food on the way.";
        }
        else if (review.score == 5)
        {
            return $"Service was stellar. With such good food and drinks, I wish we didn't reach {passenger.destination.name} so soon.";
        }

        return "Meh.";
    }

    static string ReviewB(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"This experience was terrible. I was practically left starving as we snailed away from {passenger.origin.name}.";
        }
        else if (review.score == 2)
        {
            return $"A bad experience. At least provide some snacks!";
        }
        else if (review.score == 3)
        {
            return $"Not good, not bad. Gets you from A to B.";
        }
        else if (review.score == 4)
        {
            return $"A good experience! I'd happily hire this driver again.";
        }
        else if (review.score == 5)
        {
            return $"A fantastic experience. Food and drinks were delectable.";
        }

        return "Meh.";
    }

    static string ReviewC(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"Bad, bad, bad! Keep away from {TransientDataScript.gameManager.dataManager.playerName}!";
        }
        else if (review.score == 2)
        {
            return $"I hope for {TransientDataScript.gameManager.dataManager.playerName}'s sake that {TransientDataScript.gameManager.dataManager.pronounSub.ToLower()} finds a different profession.";
        }
        else if (review.score == 3)
        {
            return $"{TransientDataScript.gameManager.dataManager.playerName} doesn't really stand out as a driver, but does {TransientDataScript.gameManager.dataManager.pronounGen.ToLower()} best.";
        }
        else if (review.score == 4)
        {
            return $"With {TransientDataScript.gameManager.dataManager.playerName}, you can expect a decent service.";
        }
        else if (review.score == 5)
        {
            return $"I cannot recommend {TransientDataScript.gameManager.dataManager.playerName} enough!";
        }

        return "Meh.";
    }

    static string ReviewD(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"Extremely uncomfortable seats, and the whole coach needs a deep clean.";
        }
        else if (review.score == 2)
        {
            return $"I think some of the food items were expired.";
        }
        else if (review.score == 3)
        {
            return $"The coach is an older model, but it gets you to your destination.";
        }
        else if (review.score == 4)
        {
            return $"Can't fault the interior. Love the retro vibe.";
        }
        else if (review.score == 5)
        {
            return $"The coach is delightfully vintage. Loved every second of my journey.";
        }

        return "Meh.";
    }

    static string ReviewE(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"Just no.";
        }
        else if (review.score == 2)
        {
            return $"Meh.";
        }
        else if (review.score == 3)
        {
            return $"Entirely okay.";
        }
        else if (review.score == 4)
        {
            return $"Great!";
        }
        else if (review.score == 5)
        {
            return $"Amazing!!!";
        }

        return "Meh.";
    }

    static string ReviewF(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"This driver is a strong argument against the Guild. Where is the quality assurance?";
        }
        else if (review.score == 2)
        {
            return $"I expected more from a Guild operator.";
        }
        else if (review.score == 3)
        {
            return $"Another average driver from the Guild of Logistics.";
        }
        else if (review.score == 4)
        {
            return $"Great service from the Guild of Logistics, as usual.";
        }
        else if (review.score == 5)
        {
            return $"This driver demonstrates what the Guild is all about!";
        }

        return "Meh.";
    }

    static string ReviewG(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"Too expensive!!";
        }
        else if (review.score == 2)
        {
            return $"Not worth the cost.";
        }
        else if (review.score == 3)
        {
            return $"You get what you pay for.";
        }
        else if (review.score == 4)
        {
            return $"Very reasonably priced, and service to match.";
        }
        else if (review.score == 5)
        {
            return $"Worth every shilling! I'd happily pay more for such stellar service.";
        }

        return "Meh.";
    }

    static string ReviewH(PassengerData passenger, PassengerReview review)
    {
        if (review.score == 1)
        {
            return $"I missed my grandmother's birthday because we took so long to reach {passenger.destination.name}.";
        }
        else if (review.score == 2)
        {
            return $"The shops in {passenger.destination.name} closed before we could get there.";
        }
        else if (review.score == 3)
        {
            return $"Made it to {passenger.destination.name} just before the pub closed.";
        }
        else if (review.score == 4)
        {
            return $"Had a great time in {passenger.destination.name} thanks to this driver's swift service!";
        }
        else if (review.score == 5)
        {
            return $"This was a new way of experiencing {passenger.destination.name}. I was fully rested and energised by the time we arrived!";
        }

        return "Meh.";
    }

    public static string GenerateComment(PassengerData passenger, PassengerReview review)
    {
        var generatorMethod = methods[Random.Range(0, methods.Count)];

        return generatorMethod(passenger, review);
    }
}