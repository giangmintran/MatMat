const uploadForm = document.getElementById('uploadForm');
const imageContainer = document.getElementById('imageContainer');
const tagFilter = document.getElementById('tagFilter');
const trashContainer = document.getElementById('trashContainer');
const trashTagFilter = document.getElementById('trashTagFilter');
let imageList = [];
let trashList = [];
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
    <button onclick="moveToTrash(${imageId})">Move to Trash</button>
  `;

  imageElement.id = imageId;
  imageContainer.appendChild(imageElement);
  imageList.push({ id: imageId, imageUrl, tags });
}

// Function to move image to trash
function moveToTrash(imageId) {
  const imageElement = document.getElementById(imageId);
  if (imageElement) {
    const imageData = imageList.find(image => image.id === imageId);
    trashList.push(imageData); // Thêm vào danh sách thùng rác
    imageList = imageList.filter(image => image.id !== imageId); // Xóa khỏi danh sách ảnh chính
    imageContainer.removeChild(imageElement);
    addToTrash(imageData); // Hiển thị trong thùng rác
  }
}

// Function to add image to trash container
function addToTrash(imageData) {
  const trashElement = document.createElement('div');
  trashElement.classList.add('image-item');
  trashElement.setAttribute('data-tags', imageData.tags.join(','));
  trashElement.innerHTML = `
    <img src="${imageData.imageUrl}" alt="Trash Image">
    <div class="tags">Tags: ${imageData.tags.join(', ')}</div>
    <button onclick="restoreImage(${imageData.id})">Restore</button>
    <button onclick="deleteImagePermanently(${imageData.id})">Delete Permanently</button>
  `;

  trashElement.id = imageData.id; // Thiết lập id để khôi phục và xóa
  trashContainer.appendChild(trashElement);
}

// Function to restore image from trash
function restoreImage(imageId) {
  const trashElement = document.getElementById(imageId);
  if (trashElement) {
    const imageData = trashList.find(image => image.id === imageId);
    trashList = trashList.filter(image => image.id !== imageId); // Xóa khỏi danh sách thùng rác
    trashContainer.removeChild(trashElement);
    addImageToGallery(imageData.imageUrl, imageData.tags); // Thêm lại vào danh sách ảnh chính
  }
}

// Function to delete image permanently
function deleteImagePermanently(imageId) {
  const trashElement = document.getElementById(imageId);
  if (trashElement) {
    trashContainer.removeChild(trashElement);
    trashList = trashList.filter(image => image.id !== imageId); // Xóa khỏi danh sách thùng rác
  }
}

// Function to add tags to the filter dropdown
function addTagsToFilter(tags) {
  tags.forEach(tag => allTags.add(tag)); // Add new tags to the set
  updateTagFilter();
  updateTrashTagFilter(tags); // Cập nhật bộ lọc thùng rác
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

// Function to update the trash tag filter dropdown
function updateTrashTagFilter(tags) {
  tags.forEach(tag => allTags.add(tag)); // Add new tags to the set
  trashTagFilter.innerHTML = '<option value="all">All</option>'; // Reset options
  allTags.forEach(tag => {
    const option = document.createElement('option');
    option.value = tag;
    option.textContent = tag;
    trashTagFilter.appendChild(option);
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

// Function to filter trash by tag
function filterTrashByTag() {
  const selectedTag = trashTagFilter.value;
  const trashImages = trashContainer.querySelectorAll('.image-item');

  trashImages.forEach(image => {
    const imageTags = image.getAttribute('data-tags').split(',');
    if (selectedTag === 'all' || imageTags.includes(selectedTag)) {
      image.style.display = 'block';
    } else {
      image.style.display = 'none';
    }
  });
}
