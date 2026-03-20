using Airport_Managment_SYS.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{  [Area ("Admin" )]
    public class ChatbotQuestionsController : Controller
    {
      private readonly IRepository<ChatbotQuestion> _chatbotQuestionsRepository;

        public ChatbotQuestionsController(IRepository<ChatbotQuestion> chatbotQuestionsRepository)
        {
            _chatbotQuestionsRepository = chatbotQuestionsRepository;
        }

        public async Task<IActionResult> Index()
        {

            var chatbot_questions= await _chatbotQuestionsRepository.GetAsync();

            return View(chatbot_questions.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var chatbot_questions = await _chatbotQuestionsRepository.GetAsync();

            return View(new CreateChatBotQuestionVM() { Questions=chatbot_questions});
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateChatBotQuestionVM createChatBotQuestionVM)
        {

            if(!ModelState.IsValid)
            {

                var chatbot_questions = await _chatbotQuestionsRepository.GetAsync();
                    createChatBotQuestionVM.Questions = chatbot_questions;
                return View(createChatBotQuestionVM);
             

            }   var chatbot_question = new ChatbotQuestion()
                {
                    Question = createChatBotQuestionVM.Question,
                    Answer = createChatBotQuestionVM.Answer,
                     ChatbotQuestionId = createChatBotQuestionVM.ParentId,
                    type = createChatBotQuestionVM.Type
                };
                await _chatbotQuestionsRepository.AddAsync(chatbot_question);
            await _chatbotQuestionsRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    
        public async Task<IActionResult> Update(int id)
        {
            var chatbot_questions = await _chatbotQuestionsRepository.GetAsync(c=>c.Id!=id);


            var chatbot_question = await _chatbotQuestionsRepository.GetOneAsync(c=>c.Id==id);
             return View(new UpdateChatbotQuestionVM() { 
                 Id= chatbot_question.Id,
                 Questions=chatbot_questions,
                 Answer=chatbot_question.Answer,
                 Question=chatbot_question.Question, 
                 Type=chatbot_question.type,
                 ParentId=chatbot_question.ChatbotQuestionId});
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateChatbotQuestionVM updateChatbotQuestionVM)
        {
            var question = await _chatbotQuestionsRepository.GetOneAsync(c=>c.Id==updateChatbotQuestionVM.Id);

            if (question == null)
                return NotFound();

            if (updateChatbotQuestionVM.Type == "Main")
            {
                updateChatbotQuestionVM.ParentId = null;
            }

            if (updateChatbotQuestionVM.Type == "Sub" && updateChatbotQuestionVM.ParentId == null)
            {
                ModelState.AddModelError("ParentId", "لازم تختار Parent");
            }

            if (!ModelState.IsValid)
            {
                updateChatbotQuestionVM.Questions = await _chatbotQuestionsRepository.GetAsync(c => c.Id != question.Id);



                return View(updateChatbotQuestionVM);
            }

            // 🔥 Update
            question.Question = updateChatbotQuestionVM.Question;
            question.Answer = updateChatbotQuestionVM.Answer;
            question.type = updateChatbotQuestionVM.Type;
            question.ChatbotQuestionId = updateChatbotQuestionVM.ParentId;

            await _chatbotQuestionsRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            var chatbot_question = await _chatbotQuestionsRepository.GetOneAsync(c=>c.Id==id);
            _chatbotQuestionsRepository.Delete(chatbot_question);
           await _chatbotQuestionsRepository.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
