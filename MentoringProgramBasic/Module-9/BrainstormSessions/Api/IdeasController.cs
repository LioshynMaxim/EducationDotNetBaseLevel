using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.ClientModels;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrainstormSessions.Api
{
    public class IdeasController : ControllerBase
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private readonly ILogger<IdeasController> _logger;

        public IdeasController(IBrainstormSessionRepository sessionRepository, ILogger<IdeasController> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        #region snippet_ForSessionAndCreate
        [HttpGet("forsession/{sessionId}")]
        public async Task<IActionResult> ForSession(int sessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("Session {SessionId} not found", sessionId);
                return NotFound(sessionId);
            }

            var result = session.Ideas.Select(idea => new IdeaDTO()
            {
                Id = idea.Id,
                Name = idea.Name,
                Description = idea.Description,
                DateCreated = idea.DateCreated
            }).ToList();

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]NewIdeaModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating idea for session {SessionId}", model?.SessionId);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new idea '{IdeaName}' for session {SessionId}", model.Name, model.SessionId);

            var session = await _sessionRepository.GetByIdAsync(model.SessionId);
            if (session == null)
            {
                _logger.LogError("Cannot create idea: Session {SessionId} not found", model.SessionId);
                return NotFound(model.SessionId);
            }

            var idea = new Idea()
            {
                DateCreated = DateTimeOffset.Now,
                Description = model.Description,
                Name = model.Name
            };
            session.AddIdea(idea);

            await _sessionRepository.UpdateAsync(session);

            _logger.LogInformation("Successfully created idea '{IdeaName}' for session {SessionId}", model.Name, model.SessionId);

            return Ok(session);
        }
        #endregion

        #region snippet_ForSessionActionResult
        [HttpGet("forsessionactionresult/{sessionId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<IdeaDTO>>> ForSessionActionResult(int sessionId)
        {
            _logger.LogDebug("ForSessionActionResult called for session {SessionId}", sessionId);
            
            var session = await _sessionRepository.GetByIdAsync(sessionId);

            if (session == null)
            {
                _logger.LogWarning("Session {SessionId} not found in ForSessionActionResult", sessionId);
                return NotFound(sessionId);
            }

            var result = session.Ideas.Select(idea => new IdeaDTO()
            {
                Id = idea.Id,
                Name = idea.Name,
                Description = idea.Description,
                DateCreated = idea.DateCreated
            }).ToList();

            _logger.LogDebug("Returning {IdeaCount} ideas for session {SessionId}", result.Count, sessionId);

            return result;
        }
        #endregion

        #region snippet_CreateActionResult
        [HttpPost("createactionresult")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BrainstormSession>> CreateActionResult([FromBody]NewIdeaModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in CreateActionResult for session {SessionId}. Errors: {ModelStateErrors}", 
                    model?.SessionId, 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            _logger.LogDebug("CreateActionResult called for session {SessionId} with idea '{IdeaName}'", model.SessionId, model.Name);

            var session = await _sessionRepository.GetByIdAsync(model.SessionId);

            if (session == null)
            {
                _logger.LogError("Cannot create idea in CreateActionResult: Session {SessionId} not found", model.SessionId);
                return NotFound(model.SessionId);
            }

            var idea = new Idea()
            {
                DateCreated = DateTimeOffset.Now,
                Description = model.Description,
                Name = model.Name
            };
            session.AddIdea(idea);

            await _sessionRepository.UpdateAsync(session);

            _logger.LogInformation("Successfully created idea '{IdeaName}' in session {SessionId} via CreateActionResult", model.Name, model.SessionId);

            return CreatedAtAction(nameof(CreateActionResult), new { id = session.Id }, session);
        }
        #endregion
    }
}
