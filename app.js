const uploadForm = document.getElementById('uploadForm');
const imageContainer = document.getElementById('imageContainer');
const tagFilter = document.getElementById('tagFilter');
let imageList = [];
let allTags = new Set(); // Set để đảm bảo các tag không bị lặp lại

// Function to handle image upload
uploadForm.addEventListener('submit', function(e) {
  e.preventDefault();
  const fileInput = document.getElementById('imageFile');
  const tagInput = document.getElementById('imageTag');

  const file = fileInput.files[0];
  const tags = tagInput.value.split(',').map(tag => tag.trim()).filter(tag => tag);

  if (file) {
    const reader = new FileReader();
    reader.onload = function(event) {
      const imageUrl = event.target.result;
      addImageToGallery(imageUrl, tags);
      addTagsToFilter(tags);  // Thêm tag mới vào bộ lọc
    };
    reader.readAsDataURL(file);
  }
});

// Function to add image to gallery
function addImageToGallery(imageUrl, tags) {
  const imageId = Date.now();
  const imageElement = document.createElement('div');
  imageElement.classList.add('image-item');
  imageElement.setAttribute('data-tags', tags.join(','));
  imageElement.innerHTML = `
    <img src="${imageUrl}" alt="Uploaded Image">
    <div class="tags">Tags: ${tags.join(', ')}</div>
    <button onclick="deleteImage(${imageId})">Delete</button>
  `;

  imageElement.id = imageId;
  imageContainer.appendChild(imageElement);
  imageList.push({ id: imageId, imageUrl, tags });
}

// Function to delete image
function deleteImage(imageId) {
  const imageElement = document.getElementById(imageId);
  if (imageElement) {
    imageContainer.removeChild(imageElement);
    imageList = imageList.filter(image => image.id !== imageId);
  }
}

// Function to add tags to the filter dropdown
function addTagsToFilter(tags) {
  tags.forEach(tag => allTags.add(tag)); // Add new tags to the set
  updateTagFilter();
}

// Function to update the tag filter dropdown
function updateTagFilter() {
  tagFilter.innerHTML = '<option value="all">All</option>'; // Reset options
  allTags.forEach(tag => {
    const option = document.createElement('option');
    option.value = tag;
    option.textContent = tag;
    tagFilter.appendChild(option);
  });
}

// Function to filter images by tag
function filterByTag() {
  const selectedTag = tagFilter.value;
  const images = document.querySelectorAll('.image-item');

  images.forEach(image => {
    const imageTags = image.getAttribute('data-tags').split(',');
    if (selectedTag === 'all' || imageTags.includes(selectedTag)) {
      image.style.display = 'block';
    } else {
      image.style.display = 'none';
    }
  });
}
