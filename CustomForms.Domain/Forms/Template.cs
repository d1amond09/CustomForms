using CustomForms.Domain.Common;
using CustomForms.Domain.Common.Constants;
using CustomForms.Domain.Common.Exceptions;
using CustomForms.Domain.Users;

namespace CustomForms.Domain.Forms;

public class Template : AuditableEntity
{
	public string Title { get; private set; }
	public string Description { get; private set; }
	public Guid AuthorId { get; private set; }
	public virtual User Author { get; private set; } = null!; 
	public Guid TopicId { get; private set; }
	public virtual Topic Topic { get; private set; } = null!; 
	public string? ImageUrl { get; private set; }
	public bool IsPublic { get; private set; } 

	private readonly List<Question> _questions = [];
	public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

	private readonly List<Tag> _tags = [];
	public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

	private readonly List<User> _allowedUsers = [];
	public IReadOnlyCollection<User> AllowedUsers => _allowedUsers.AsReadOnly();

	private readonly List<Comment> _comments = [];
	public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

	private readonly List<Like> _likes = [];
	public IReadOnlyCollection<Like> Likes => _likes.AsReadOnly();

	public virtual ICollection<Form> Forms { get; private set; } = new List<Form>();


	public Template(Guid id, string title, string description, Guid authorId, Guid topicId, bool isPublic = true, string? imageUrl = null) : base(id)
	{
		if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Template title cannot be empty.", nameof(title));
		if (authorId == Guid.Empty) throw new ArgumentException("Author ID cannot be empty.", nameof(authorId));
		if (topicId == Guid.Empty) throw new ArgumentException("Topic ID cannot be empty.", nameof(topicId));

		Title = title;
		Description = description; 
		AuthorId = authorId;
		TopicId = topicId;
		IsPublic = isPublic;
		ImageUrl = imageUrl;
	}

	public void UpdateDetails(string title, string description, Guid topicId, string? imageUrl)
	{
		if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Template title cannot be empty.", nameof(title));
		if (topicId == Guid.Empty) throw new ArgumentException("Topic ID cannot be empty.", nameof(topicId));

		Title = title;
		Description = description;
		TopicId = topicId;
		ImageUrl = imageUrl; 
		SetModifiedDate();
	}

	public void SetAccess(bool isPublic, IEnumerable<User>? allowedUsers = null)
	{
		IsPublic = isPublic;
		_allowedUsers.Clear();
		if (!isPublic && allowedUsers != null)
		{
			foreach (var user in allowedUsers)
			{
				if (user.Id == AuthorId) continue;
				if (!_allowedUsers.Any(u => u.Id == user.Id)) 
					_allowedUsers.Add(user);
			}
		}
		SetModifiedDate();
	}

	public void AddAllowedUser(User user)
	{
		if (!IsPublic && user.Id != AuthorId && !_allowedUsers.Any(u => u.Id == user.Id))
		{
			_allowedUsers.Add(user);
			SetModifiedDate();
		}
	}

	public void RemoveAllowedUser(Guid userId)
	{
		var userToRemove = _allowedUsers.FirstOrDefault(u => u.Id == userId);
		if (userToRemove != null)
		{
			_allowedUsers.Remove(userToRemove);
			SetModifiedDate();
		}
	}

	public void AddQuestion(Question question)
	{
		int countOfType = _questions.Count(q => q.Type == question.Type);
		int limit = question.Type switch
		{
			QuestionType.String => DomainConstants.MaxStringQuestions,
			QuestionType.Text => DomainConstants.MaxTextQuestions,
			QuestionType.Integer => DomainConstants.MaxIntegerQuestions,
			QuestionType.Checkbox => DomainConstants.MaxCheckboxQuestions,
			_ => 0 
		};
		if (countOfType >= limit)
		{
			throw new QuestionLimitExceededException(question.Type, limit);
		}

		question.SetOrder(_questions.Count);
		_questions.Add(question);
		SetModifiedDate();
	}

	public void RemoveQuestion(Guid questionId)
	{
		var question = _questions.FirstOrDefault(q => q.Id == questionId);
		if (question != null)
		{
			_questions.Remove(question);
			ReorderQuestions(_questions.OrderBy(q => q.Order).Select(q => q.Id).ToList());
			SetModifiedDate();
		}
	}

	public void UpdateQuestion(Guid questionId, string title, string description, bool showInResults)
	{
		var question = _questions.FirstOrDefault(q => q.Id == questionId);
		if (question != null)
		{
			question.UpdateDetails(title, description, showInResults);
			SetModifiedDate();
		}
		else
		{
			throw new EntityNotFoundException(nameof(Question), questionId);
		}
	}

	public void ReorderQuestions(List<Guid> orderedQuestionIds)
	{
		if (orderedQuestionIds.Count != _questions.Count || orderedQuestionIds.Distinct().Count() != _questions.Count)
		{
			throw new InvalidQuestionOrderException("Invalid list of question IDs for reordering.");
		}

		var questionsDict = _questions.ToDictionary(q => q.Id);
		if (orderedQuestionIds.Any(id => !questionsDict.ContainsKey(id)))
		{
			throw new InvalidQuestionOrderException("List contains non-existent question IDs.");
		}

		int order = 0;
		foreach (var id in orderedQuestionIds)
		{
			questionsDict[id].SetOrder(order++);
		}
		_questions.Sort((a, b) => a.Order.CompareTo(b.Order));

		SetModifiedDate();
	}

	public void AddTag(Tag tag)
	{
		if (!_tags.Any(t => t.Id == tag.Id))
		{
			_tags.Add(tag);
			SetModifiedDate();
		}
	}

	public void SetTags(IEnumerable<Tag> tags)
	{
		_tags.Clear();
		foreach (var tag in tags.DistinctBy(t => t.Id))
		{
			_tags.Add(tag);
		}
		SetModifiedDate();
	}

	public void RemoveTag(Guid tagId)
	{
		var tag = _tags.FirstOrDefault(t => t.Id == tagId);
		if (tag != null)
		{
			_tags.Remove(tag);
			SetModifiedDate();
		}
	}

	public void AddComment(Comment comment)
	{
		if (comment.TemplateId != this.Id) throw new Exception("Comment does not belong to this template.");
		_comments.Add(comment);
		SetModifiedDate(); // Template itself is modified when comments change
	}

	public void RemoveComment(Guid commentId, Guid userId, bool isAdmin)
	{
		var comment = _comments.FirstOrDefault(c => c.Id == commentId);
		if (comment != null)
		{
			// Only author or admin can delete
			if (comment.UserId == userId || isAdmin)
			{
				_comments.Remove(comment);
				SetModifiedDate();
			}
			else
			{
				throw new AuthorizationException("User not authorized to delete this comment.");
			}
		}
	}

	public void AddLike(Like like)
	{
		if (like.TemplateId != this.Id) throw new Exception("Like does not belong to this template.");
		if (!_likes.Any(l => l.UserId == like.UserId))
		{
			_likes.Add(like);
			SetModifiedDate();
		}
	}

	public void RemoveLike(Guid userId)
	{
		var like = _likes.FirstOrDefault(l => l.UserId == userId);
		if (like != null)
		{
			_likes.Remove(like);
			SetModifiedDate();
		}
	}

	public bool CanUserView(User? user)
	{
		if (IsPublic) return true;
		if (user == null) return false; 
		if (user.IsAdmin()) return true;
		if (user.Id == AuthorId) return true;
		return _allowedUsers.Any(u => u.Id == user.Id);
	}

	public bool CanUserFill(User? user)
	{
		if (user == null || user.IsBlocked) return false;
		if (IsPublic) return true;
		if (user.Id == AuthorId) return false; 
		return _allowedUsers.Any(u => u.Id == user.Id);
	}

	public bool CanUserManage(User? user)
	{
		if (user == null) return false;
		if (user.IsAdmin()) return true;
		return user.Id == AuthorId;
	}
}
