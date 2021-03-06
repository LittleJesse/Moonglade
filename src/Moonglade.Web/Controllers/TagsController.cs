﻿using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moonglade.Core;
using Moonglade.Web.Filters;

namespace Moonglade.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("names")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Names()
        {
            var tagNames = await _tagService.GetAllNamesAsync();
            return Ok(tagNames);
        }

        [HttpPost("update")]
        [TypeFilter(typeof(ClearPagingCountCache))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(EditTagRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _tagService.UpdateAsync(request.TagId, request.NewName);
            return Ok();
        }

        [HttpDelete("{tagId}")]
        [TypeFilter(typeof(ClearPagingCountCache))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int tagId)
        {
            if (tagId is <= 0 or > 9999)
            {
                ModelState.AddModelError(nameof(tagId), "Value out of range");
                return BadRequest(ModelState);
            }

            await _tagService.DeleteAsync(tagId);
            return Ok();
        }
    }

    public class EditTagRequest
    {
        [Range(1, 9999)]
        public int TagId { get; set; }

        [Required]
        public string NewName { get; set; }
    }
}