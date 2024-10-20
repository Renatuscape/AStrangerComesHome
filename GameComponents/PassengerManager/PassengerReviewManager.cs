using UnityEngine;
using Random = UnityEngine.Random;

public static class PassengerReviewManager
{

    public static void RollForReview(PassengerData passenger)
    {
        if (passenger.satisfaction < 1f || passenger.satisfaction > 9) // Guarantee a review at extreme satisfaction or dissatisfaction
        {
            PostReview(passenger);
        }
        else if (passenger.leavesReview) // Guarantee a review if the character was initially set to leave a review on boarding
        {
            PostReview(passenger);
        }
        else // If the character is not set to leave a review, allow a small reroll chance in case satisfaction is very high or very low
        {
            float rerollChance = Mathf.Abs(passenger.satisfaction - 5) * 2f;

            if (Random.Range(0, 100) < rerollChance)
            {
                PostReview(passenger);
            }
        }
    }
    
    static void PostReview(PassengerData passenger)
    {
        PassengerReview review = new();

        review.score = Mathf.Clamp(Mathf.CeilToInt(passenger.satisfaction / 2), 1, 5);
        review.passengerName = passenger.passengerName;
        review.originID = passenger.origin.objectID;
        review.destinationID = passenger.destination.objectID;
        review.comment = PassengerCommentGenerator.GenerateComment(passenger, review);
        review.time = TransientDataScript.gameManager.dataManager.totalGameDays;

        TransientDataScript.gameManager.dataManager.reviews.Add(review);
        LogAlert.QueueTextAlert($"{passenger.passengerName} left a {review.score} star review.");

        string reviewID = $"SCR02{review.score}"; // Review scripts are numbered 21 - 25 and should correspond to the score value
        Player.Add(reviewID);
    }
}
