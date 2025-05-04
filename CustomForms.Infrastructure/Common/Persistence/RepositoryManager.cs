using CustomForms.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using CustomForms.Domain.Users;
using CustomForms.Infrastructure.Templates.Persistence;
using CustomForms.Infrastructure.Forms.Persistence;
using CustomForms.Infrastructure.Users.Persistence;

namespace CustomForms.Infrastructure.Common.Persistence;

public class RepositoryManager : IRepositoryManager
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<User> _userManager;

	private readonly Lazy<IUserRepository> _userRepository;
	private readonly Lazy<ITemplateRepository> _templateRepository;
	private readonly Lazy<IFormRepository> _formRepository;
	private readonly Lazy<ICommentRepository> _commentRepository;
	private readonly Lazy<IQuestionRepository> _questionRepository;
	private readonly Lazy<ILikeRepository> _likeRepository;
	private readonly Lazy<ITagRepository> _tagRepository;
	private readonly Lazy<ITopicRepository> _topicRepository;

	public RepositoryManager(AppDbContext dbContext, UserManager<User> userManager)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

		_userRepository = new Lazy<IUserRepository>(() => new UserRepository(_userManager, _dbContext));
		_templateRepository = new Lazy<ITemplateRepository>(() => new TemplateRepository(_dbContext));
		_formRepository = new Lazy<IFormRepository>(() => new FormRepository(_dbContext)); 
		_commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(_dbContext));
		_questionRepository = new Lazy<IQuestionRepository>(() => new QuestionRepository(_dbContext));
		_likeRepository = new Lazy<ILikeRepository>(() => new LikeRepository(_dbContext));      
		_tagRepository = new Lazy<ITagRepository>(() => new TagRepository(_dbContext));
		_topicRepository = new Lazy<ITopicRepository>(() => new TopicRepository(_dbContext)); 
	}

	public IUserRepository Users => _userRepository.Value;
	public ITemplateRepository Templates => _templateRepository.Value;
	public IFormRepository Forms => _formRepository.Value;
	public ICommentRepository Comments => _commentRepository.Value;
	public IQuestionRepository Questions => _questionRepository.Value;
	public ILikeRepository Likes => _likeRepository.Value;
	public ITagRepository Tags => _tagRepository.Value;
	public ITopicRepository Topics => _topicRepository.Value;


	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
	{
		return await _dbContext.SaveChangesAsync(cancellationToken);
	}

	private bool disposed = false;

	protected virtual void Dispose(bool disposing)
	{
		if (!this.disposed)
		{
			if (disposing)
			{
				_dbContext.Dispose(); 
			}
		}
		this.disposed = true;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
