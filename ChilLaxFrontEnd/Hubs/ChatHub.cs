using ChilLaxFrontEnd.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace CoreMVC_SignalR_Chat.Hubs
{
    public class ChatHub : Hub
    {
        
        
        

        public async Task SendUserMessage(string selfID, string message)
        {
            await Clients.All.SendAsync("UpdContent", selfID + " 你說: " + message);
        }

        public async Task SendAIBotReply(string threeCards, string message)
        {
            // 使用 GPT 機器人獲取回應
            var api = new OpenAI_API.OpenAIAPI("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");
            // 提取 GPT 機器人的回應
            var gptResponse = string.Empty;
            await api.Completions.StreamCompletionAsync(
                new CompletionRequest(
                    message + $"你是一個塔羅牌占卜師，你要很熱情地回覆我想要詢問的事情，你負責幫所有人占卜任何塔羅牌相關的問題並依照這三張牌{threeCards}去回答使用者詢問的任何問題",
                    model: Model.DavinciText,
                    max_tokens: 2048,
                    temperature: 0.5,
                    presencePenalty: 0.1,
                    frequencyPenalty: 0.1),
                res => gptResponse += res.ToString());

            // 將 GPT 機器人的回應發送到聊天室
            await Clients.All.SendAsync("UpdContent", "Chillen說:" + gptResponse);
        }


        
        

        /// <summary>
        /// 傳遞訊息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SendMessage(string threeCards, string selfID, string message, string sendToID)
        {
            if (string.IsNullOrEmpty(sendToID))
            {
                await SendUserMessage(selfID, message);
            }
            else
            {
                // 可以在這裡寫傳送給特定用戶的邏輯，如果需要的話
            }

            //傳送訊息給AI占卜師
           await SendAIBotReply(threeCards, message);
        }







        

    }
}